using Labirint.Core.Interfaces;
using Labirint.Core.TileFeatures;

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
        _parameters = new(seeder, width, height, density);

        var length = width * height - 1;

        var totalItemsCount = FillItemCounts(placeableItems);

        if (totalItemsCount > length)
        {
            ReduceItemCounts(length, totalItemsCount, width, height, density);
        }

        EnqueueRequiredItems();

        var indexes = ShuffleIndexes(length);
        PlaceItemsOnMap(width, indexes);
    }

    private int FillItemCounts(IEnumerable<Item> placeableItems)
    {
        if (_parameters == null)
        {
            throw new($"Невозможно сосчитать количество предметов из-за отсутствующий {nameof(WorldItemParameters)}");
        }

        var totalItemsCount = 0;

        foreach (var item in placeableItems)
        {
            var count = item.CalculateCountInMaze(_parameters.Width, _parameters.Height, _parameters.Density);
            _itemCounts[item] = count;
            totalItemsCount += count;
        }

        return totalItemsCount;
    }

    private void ReduceItemCounts(int length, int totalItemsCount, int width, int height, int density)
    {
        var reductionFactor = (double)length / totalItemsCount;
        var reducedItemCount = 0;

        foreach (var (item, count) in _itemCounts)
        {
            var reducedCount = (int)Math.Floor(count * reductionFactor);
            reducedItemCount += reducedCount;
            _itemCounts[item] = reducedCount;
        }

        foreach (var (item, count) in _itemCounts)
        {
            if (length - reducedItemCount <= 0)
            {
                break;
            }

            var maxCount = item.CalculateCountInMaze(width, height, density);

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
            throw new($"Невозможно добавить предметы в очередь из-за отсутствующий {nameof(WorldItemParameters)}");
        }

        foreach (var (item, count) in _itemCounts)
        {
            foreach (var worldItem in item.GetItemsForPlace(_parameters).Take(count))
            {
                _requiredItems.Enqueue(worldItem);
            }
        }
    }

    private int[] ShuffleIndexes(int length)
    {
        var indexes = Enumerable.Range(1, length).ToArray();

        for (var i = 0; i < _requiredItems.Count - 1; i++)
        {
            var j = seeder.Generator.Next(i + 1, length);
            (indexes[i], indexes[j]) = (indexes[j], indexes[i]);
        }

        return indexes;
    }

    private void PlaceItemsOnMap(int width, int[] indexes)
    {
        var placingItemsCount = _requiredItems.Count;

        for (var i = 0; i < placingItemsCount && i < indexes.Length; i++)
        {
            var index = indexes[i];
            var x = index % width;
            var y = index / width;

            if (_requiredItems.TryDequeue(out var placeable))
            {
                placeItemAction.Invoke(x, y, placeable);
            }
        }
    }
}
