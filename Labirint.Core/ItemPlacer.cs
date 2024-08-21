using Labirint.Core.Interfaces;

namespace Labirint.Core;

public class ItemPlacer(IRandom seeder, Action<int, int, WorldItem> placeItemAction)
{
    private readonly Dictionary<Item, int> _itemCounts = new();
    private readonly Queue<WorldItem> _requiredItems = new();
    private WorldItemParameters? _parameters;

    public void PlaceItems(int width, int height, int density, IEnumerable<Item> placeableItems)
    {
        _itemCounts.Clear();
        _requiredItems.Clear();
        _parameters = new WorldItemParameters(seeder, width, height, density);

        int length = width * height - 1;

        int totalItemsCount = FillItemCounts(placeableItems);

        if (totalItemsCount > length)
        {
            ReduceItemCounts(length, totalItemsCount, width, height, density);
        }

        EnqueueRequiredItems();

        int[] indexes = ShuffleIndexes(length);
        PlaceItemsOnMap(width, indexes);
    }

    private int FillItemCounts(IEnumerable<Item> placeableItems)
    {
        if (_parameters == null)
        {
            throw new Exception($"Невозможно сосчитать количество предметов из-за отсутствующий {nameof(WorldItemParameters)}");
        }

        int totalItemsCount = 0;

        foreach (Item item in placeableItems)
        {
            int count = item.GetItemsForPlace(_parameters).Count();
            _itemCounts[item] = count;
            totalItemsCount += count;
        }

        return totalItemsCount;
    }

    private void ReduceItemCounts(int length, int totalItemsCount, int width, int height, int density)
    {
        double reductionFactor = (double)length / totalItemsCount;
        int reducedItemCount = 0;

        foreach ((Item item, int count) in _itemCounts)
        {
            int reducedCount = (int)Math.Floor(count * reductionFactor);
            reducedItemCount += reducedCount;
            _itemCounts[item] = reducedCount;
        }

        foreach ((Item item, int count) in _itemCounts)
        {
            if (length - reducedItemCount <= 0)
            {
                break;
            }

            int maxCount = item.CalculateCountInMaze(width, height, density);

            if (maxCount <= count)
            {
                continue;
            }

            _itemCounts[item]++;
            reducedItemCount++;
        }
    }

    private void EnqueueRequiredItems()
    {
        if (_parameters == null)
        {
            throw new Exception($"Невозможно добавить предметы в очередь из-за отсутствующий {nameof(WorldItemParameters)}");
        }

        foreach ((Item item, int count) in _itemCounts)
        {
            foreach (WorldItem worldItem in item.GetItemsForPlace(_parameters).Take(count))
            {
                _requiredItems.Enqueue(worldItem);
            }
        }
    }

    private int[] ShuffleIndexes(int length)
    {
        int[] indexes = Enumerable.Range(1, length).ToArray();

        for (int i = 0; i < _requiredItems.Count - 1; i++)
        {
            int j = seeder.Random.Next(i + 1, length);
            (indexes[i], indexes[j]) = (indexes[j], indexes[i]);
        }

        return indexes;
    }

    private void PlaceItemsOnMap(int width, int[] indexes)
    {
        int placingItemsCount = _requiredItems.Count;

        for (int i = 0; i < placingItemsCount && i < indexes.Length; i++)
        {
            int index = indexes[i];
            int x = index / width;
            int y = index % width;

            if (_requiredItems.TryDequeue(out WorldItem? placeable))
            {
                placeItemAction.Invoke(x, y, placeable);
            }
        }
    }
}
