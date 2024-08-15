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

        PlaceItems(width, height, density, placeableItems);
    }

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

    public event EventHandler? ExitFound;
    public event EventHandler<Position>? PlayerMoved;
    public event EventHandler<WorldItem>? ItemPickedUp;

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

    private void PlaceItems(int width, int height, int density, IEnumerable<Item> placeableItems)
    {
        int length = width * height - 1;
        Dictionary<Item, int> itemCounts = new();
        Queue<WorldItem> requiredItems = new();
        int totalItemsCount = 0;

        foreach (Item item in placeableItems)
        {
            int count = item.GetItemsForPlace(width, height, density).Count();
            itemCounts[item] = count;
            totalItemsCount += count;
        }

        if (totalItemsCount > length)
        {
            double reductionFactor = (double)length / totalItemsCount;

            foreach ((Item? key, int value) in itemCounts)
            {
                int reducedCount = (int)Math.Round(value * reductionFactor);
                itemCounts[key] = reducedCount;
            }
        }

        foreach ((Item? item, int count) in itemCounts)
        {
            foreach (WorldItem worldItem in item.GetItemsForPlace(width, height, density).Take(count))
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
            }
        }
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
