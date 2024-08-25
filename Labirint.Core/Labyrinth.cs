using Labirint.Core.Extensions;
using Labirint.Core.Interfaces;

namespace Labirint.Core;

/// <summary>
///     Лабиринт.
/// </summary>
public class Labyrinth
{
    private readonly IRandom _seeder;
    private readonly ItemPlacer _itemPlacer;

    public Labyrinth(IRandom seeder)
    {
        _seeder = seeder;

        _itemPlacer = new ItemPlacer(_seeder, (x, y, item) =>
        {
            this[x, y].AddFeature(item);
            item.AfterPlace.Invoke((x, y), this);
        });
    }

    /// <summary>
    ///     Событие, которое вызывается, когда игрок находит выход из лабиринта.
    /// </summary>
    public event EventHandler? ExitFound;

    /// <summary>
    ///     Событие, которое вызывается, когда игрок подбирает предмет.
    /// </summary>
    public event EventHandler<TileFeature>? ItemPickedUp;

    /// <summary>
    ///     Событие, которое вызывается, когда игрок успешно перемещается.
    /// </summary>
    public event EventHandler<Position>? RunnerMoved;

    /// <summary>
    ///     Ширина лабиринта.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    ///     Высота лабиринта.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    ///     Бегущий.
    /// </summary>
    public Runner Runner { get; private set; } = null!;

    private Tile[,] Tiles { get; set; } = null!;

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
    public void Init(int width, int height, int density, IEnumerable<Item>? placeableItems = null)
    {
        Runner = new Runner((0, 0), this);

        placeableItems ??= Runner.Inventory.AllItems;

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

        _itemPlacer.PlaceItems(width, height, density, placeableItems);
    }

    /// <summary>
    ///     Переместить игрока в указанном направлении.
    /// </summary>
    /// <param name="direction">Направление перемещения</param>
    public void Move(Direction direction)
    {
        if (direction == Direction.All || this[Runner.Position].ContainsWall(direction))
        {
            return;
        }

        Runner.Move(direction);
        RunnerMoved?.Invoke(this, Runner.Position);

        Tile tile = this[Runner.Position];

        if (tile.IsExit)
        {
            ExitFound?.Invoke(this, EventArgs.Empty);
        }

        if (tile.TryPickUp(out TileFeature? item))
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
        BreakWall(Runner.Position, direction);
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
        if (direction == Direction.All)
        {
            BreakWall(position, Direction.Left, Direction.Top, Direction.Right, Direction.Bottom);
            return;
        }

        if (IsCorrectPosition(position) == false)
        {
            return;
        }

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
    /// <param name="density">Плотность стены (вероятность ее создания)</param>
    /// <param name="directions">Направления, в которых нужно создать стены</param>
    public void CreateWall(Position position, int density = 100, params Direction[] directions)
    {
        foreach (Direction direction in directions)
        {
            CreateWall(position, direction, density);
        }
    }

    /// <summary>
    ///     Создать стену в указанной позиции в указанном направлении с заданной плотностью.
    /// </summary>
    /// <param name="position">Позиция, в которой нужно создать стену</param>
    /// <param name="wallDirection">Направление, в котором нужно создать стену</param>
    /// <param name="density">Плотность стены (вероятность ее создания)</param>
    public void CreateWall(Position position, Direction wallDirection, int density = 100)
    {
        if (wallDirection == Direction.All)
        {
            CreateWall(position, density, Direction.Left, Direction.Top, Direction.Right, Direction.Bottom);
            return;
        }

        if (IsCorrectPosition(position) == false)
        {
            return;
        }

        if (density != 100)
        {
            if (_seeder.Generator.Next(0, 100) >= density)
            {
                return;
            }

            if (CanAddWithoutLoop(position, wallDirection) == false)
            {
                return;
            }
        }

        this[position].AddWall(wallDirection);

        if (IsInBound(position, wallDirection) == false)
        {
            return;
        }

        PerformActionForAdjacent(position, wallDirection, (adjacentTile, oppositeDirection) => adjacentTile.AddWall(oppositeDirection));
    }

    /// <summary>
    ///     Проверяет, является ли указанная позиция корректной внутри лабиринта.
    /// </summary>
    /// <param name="position">Позиция для проверки</param>
    /// <returns>True, если позиция корректна, иначе False</returns>
    public bool IsCorrectPosition(Position position)
    {
        return position >= (0, 0) && position < (Width, Height);
    }

    private bool CanAddWithoutLoop(Position position, Direction direction)
    {
        Tile tile = this[position];

        if (tile.CanAddWall(direction) == false)
        {
            return false;
        }

        Position adjacentPosition = direction.GetAdjacentPosition(position);
        Direction oppositeDirection = direction.GetOppositeDirection();

        Tile adjacentTile = this[adjacentPosition];

        return adjacentTile.CanAddWall(oppositeDirection);
    }

    private Tile AddTile(Position position)
    {
        Tile tile = new(this);

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
