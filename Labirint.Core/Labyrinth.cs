using Labirint.Core.Extensions;
using Labirint.Core.Interfaces;

namespace Labirint.Core;

/// <summary>
///     Лабиринта.
/// </summary>
public class Labyrinth(IRandom seeder)
{
    /// <summary>
    ///     Клеточки.
    /// </summary>
    private Tile[,] Tiles { get; set; }

    public Tile this[int x, int y]
    {
        get => Tiles[x, y];
        private set => Tiles[x, y] = value;
    }

    public Tile this[Position position]
    {
        get => Tiles[position.X, position.Y];
        private set => Tiles[position.X, position.Y] = value;
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int SandCount { get; private set; }

    public Position Player { get; private set; }

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

                CreateWall((x, y), tile, Direction.Top, density);
                CreateWall((x, y), tile, Direction.Left, density);
            }
        }

        this[width / 2, height - 1].IsExit = true;
        this[width / 2, height - 1].RemoveWall(Direction.Bottom);

        // Делаем коррекцию кол-ва песочков, чтобы оно было числом размещенных песочков на поле
        SandCount = PlaceItems(width, height, density, placeableItems);
    }

    public void Move(Direction direction)
    {
        if (this[Player].ContainsWall(direction))
        {
            return;
        }

        Player += direction.ToPosition();
    }

    public void BreakWall(Direction direction)
    {
        BreakWall(Player, direction);
    }

    public void BreakWall(Position position, Direction direction)
    {
        if (IsInBound(position, direction) == false)
        {
            return;
        }

        this[position].RemoveWall(direction);

        PerformActionForAdjacent(position, direction, (adjacentTile, oppositeDirection) => adjacentTile.RemoveWall(oppositeDirection));
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

    private int PlaceItems(int width, int height, int density, IEnumerable<Item> placeableItems)
    {
        int length = width * height - 1;
        Queue<WorldItem> requiredItems = new();
        int totalItemsCount = 0;

        foreach (Item item in placeableItems)
        {
            foreach (WorldItem worldItem in item.GetItemsForPlace(width, height, density))
            {
                requiredItems.Enqueue(worldItem);
                totalItemsCount++;
            }
        }

        // TODO придумать механизм для равномерного уменьшения, а не обрезания
        // int placingSandCount = totalItemsCount > length ? length : totalItemsCount;
        int placingSandCount = totalItemsCount;

        // Исключаем клетку с игроком
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
            }
        }

        return placingSandCount;
    }

    private void CreateWall(Position position, Tile tile, Direction wallDirection, int density)
    {
        if (seeder.Random.Next(0, 100) >= density)
        {
            return;
        }

        tile.AddWall(wallDirection);

        if (IsInBound(position, wallDirection) == false)
        {
            return;
        }

        PerformActionForAdjacent(position, wallDirection, (adjacentTile, oppositeDirection) => adjacentTile.AddWall(oppositeDirection));
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
