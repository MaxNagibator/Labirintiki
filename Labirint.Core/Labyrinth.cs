using Labirint.Core.Extensions;
using Labirint.Core.Interfaces;

namespace Labirint.Core;

/// <summary>
///     Лабиринт.
/// </summary>
public class Labyrinth(IRandom seeder)
{
    /// <summary>
    ///     Событие, которое вызывается, когда игрок находит выход из лабиринта.
    /// </summary>
    public event EventHandler? ExitFound;

    /// <summary>
    ///     Событие, которое вызывается, когда игрок подбирает предмет.
    /// </summary>
    public event EventHandler<WorldItem>? ItemPickedUp;

    /// <summary>
    ///     Событие, которое вызывается, когда игрок успешно перемещается.
    /// </summary>
    public event EventHandler<Position>? PlayerMoved;

    /// <summary>
    ///     Ширина лабиринта.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    ///     Высота лабиринта.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    ///     Текущая позиция игрока.
    /// </summary>
    public Position Player { get; private set; }

    private Tile[,] Tiles { get; set; }

    /// <summary>
    ///     Клетка лабиринта по координатам.
    /// </summary>
    public Tile this[int x, int y]
    {
        get => Tiles[x, y];
        private set => Tiles[x, y] = value;
    }

    /// <summary>
    ///     Клетка лабиринта по позиции.
    /// </summary>
    public Tile this[Position position]
    {
        get => Tiles[position.X, position.Y];
        private set => Tiles[position.X, position.Y] = value;
    }

    /// <summary>
    ///     Инициализировать лабиринт с заданными параметрами.
    /// </summary>
    /// <param name="width">Ширина лабиринта</param>
    /// <param name="height">Высота лабиринта</param>
    /// <param name="density">Плотность стен в лабиринте</param>
    /// <param name="placeableItems">Список предметов, которые нужно разместить в лабиринте</param>
    public void Init(int width, int height, int density, IEnumerable<Item> placeableItems)
    {
        Player = (0, 0);
        Width = width;
        Height = height;

        Tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = AddTile((x, y));

                AddBorderWalls(x, y, tile);

                CreateWall((x, y), Direction.Top, density);
                CreateWall((x, y), Direction.Left, density);
            }
        }

        this[width / 2, height - 1].IsExit = true;
        this[width / 2, height - 1].RemoveWall(Direction.Bottom);

        PlaceItems(width, height, density, placeableItems);
    }

    /// <summary>
    ///     Переместить игрока в указанном направлении.
    /// </summary>
    /// <param name="direction">Направление перемещения</param>
    public void Move(Direction direction)
    {
        if (this[Player].ContainsWall(direction))
        {
            return;
        }

        Player += direction.ToPosition();
        PlayerMoved?.Invoke(this, Player);

        Tile tile = this[Player];

        if (tile.IsExit)
        {
            ExitFound?.Invoke(this, EventArgs.Empty);
        }

        if (tile.TryPickUp(out WorldItem? item))
        {
            ItemPickedUp?.Invoke(this, item!);
        }
    }

    /// <summary>
    ///     Разрушить стену в текущей позиции игрока в указанном направлении.
    /// </summary>
    /// <param name="direction">Направление, в котором нужно разрушить стену</param>
    public void BreakWall(Direction direction)
    {
        BreakWall(Player, direction);
    }

    /// <summary>
    ///     Разрушить стены в указанной позиции в указанных направлениях.
    /// </summary>
    /// <param name="position">Позиция, в которой нужно разрушить стены</param>
    /// <param name="directions">Направления, в которых нужно разрушить стены</param>
    public void BreakWall(Position position, params Direction[] directions)
    {
        foreach (Direction direction in directions)
        {
            BreakWall(position, direction);
        }
    }

    /// <summary>
    ///     Разрушить стену в указанной позиции в указанном направлении.
    /// </summary>
    /// <param name="position">Позиция, в которой нужно разрушить стену</param>
    /// <param name="direction">Направление, в котором нужно разрушить стену</param>
    public void BreakWall(Position position, Direction direction)
    {
        if (IsInBound(position, direction) == false)
        {
            return;
        }

        this[position].RemoveWall(direction);

        PerformActionForAdjacent(position, direction, (adjacentTile, oppositeDirection) => adjacentTile.RemoveWall(oppositeDirection));
    }

    /// <summary>
    ///     Создать стену в указанной позиции в указанных направлениях.
    /// </summary>
    /// <param name="position">Позиция, в которой нужно создать стены</param>
    /// <param name="directions">Направления, в которых нужно создать стены</param>
    public void CreateWall(Position position, params Direction[] directions)
    {
        foreach (Direction direction in directions)
        {
            CreateWall(position, direction, 100);
        }
    }

    /// <summary>
    ///     Создать стену в указанной позиции в указанном направлении с заданной плотностью.
    /// </summary>
    /// <param name="position">Позиция, в которой нужно создать стену</param>
    /// <param name="wallDirection">Направление, в котором нужно создать стену</param>
    /// <param name="density">Плотность стены (вероятность ее создания)</param>
    public void CreateWall(Position position, Direction wallDirection, int density)
    {
        if (seeder.Random.Next(0, 100) >= density)
        {
            return;
        }

        this[position].AddWall(wallDirection);

        if (IsInBound(position, wallDirection) == false)
        {
            return;
        }

        PerformActionForAdjacent(position, wallDirection, (adjacentTile, oppositeDirection) => adjacentTile.AddWall(oppositeDirection));
    }

    private Tile AddTile(Position position)
    {
        Tile tile = new();

        this[position] = tile;
        return tile;
    }

    private void AddBorderWalls(int x, int y, Tile tile)
    {
        if (x == 0)
        {
            tile.AddWall(Direction.Left);
        }

        if (x == Width - 1)
        {
            tile.AddWall(Direction.Right);
        }

        if (y == 0)
        {
            tile.AddWall(Direction.Top);
        }

        if (y == Height - 1)
        {
            tile.AddWall(Direction.Bottom);
        }
    }

    private void PlaceItems(int width, int height, int density, IEnumerable<Item> placeableItems)
    {
        int length = width * height - 1;
        Dictionary<Item, int> itemCounts = new();
        Queue<WorldItem> requiredItems = new();
        int totalItemsCount = 0;

        WorldItemParameters parameters = new(seeder, width, height, density);

        foreach (Item item in placeableItems)
        {
            int count = item.GetItemsForPlace(parameters).Count();
            itemCounts[item] = count;
            totalItemsCount += count;
        }

        if (totalItemsCount > length)
        {
            double reductionFactor = (double)length / totalItemsCount;

            foreach ((Item? key, int value) in itemCounts)
            {
                int reducedCount = (int)Math.Floor(value * reductionFactor);
                itemCounts[key] = reducedCount;
            }
        }

        foreach ((Item? item, int count) in itemCounts)
        {
            foreach (WorldItem worldItem in item.GetItemsForPlace(parameters).Take(count))
            {
                requiredItems.Enqueue(worldItem);
            }
        }

        int placingSandCount = requiredItems.Count;

        int[] indexes = Enumerable.Range(1, length).ToArray();

        for (int i = 0; i < placingSandCount; i++)
        {
            int j = seeder.Random.Next(i + 1, length);
            (indexes[i], indexes[j]) = (indexes[j], indexes[i]);
        }

        for (int i = 0; i < placingSandCount; i++)
        {
            int index = indexes[i];
            int x = index / width;
            int y = index % width;

            if (requiredItems.TryDequeue(out WorldItem? placeable))
            {
                this[x, y].WorldItem = placeable;
                placeable.AfterPlace.Invoke((x, y), this);
            }
        }
    }

    private void PerformActionForAdjacent(Position position, Direction wallDirection, Action<Tile, Direction> action)
    {
        Position adjacentPosition = wallDirection.GetAdjacentPosition(position);
        Direction oppositeDirection = wallDirection.GetOppositeDirection();
        action.Invoke(this[adjacentPosition], oppositeDirection);
    }

    private bool IsInBound(Position position, Direction direction)
    {
        return direction switch
        {
            Direction.Left => position.X > 0,
            Direction.Top => position.Y > 0,
            Direction.Right => position.X < Width - 1,
            Direction.Bottom => position.Y < Height - 1,
            var _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}
