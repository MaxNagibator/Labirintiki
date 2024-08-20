using Labirint.Core.Interfaces;

namespace Labirint.Core;

public class ItemPlacer(IRandom seeder, Action<int, int, WorldItem> placeItemAction)
{
    public void PlaceItems(int width, int height, int density, IEnumerable<Item> placeableItems)
    {
        int length = width * height - 1;

        Dictionary<Item, int> itemCounts = new();
        Queue<WorldItem> requiredItems = new();

        WorldItemParameters parameters = new(seeder, width, height, density);

        int totalItemsCount = CountTotalItems(placeableItems, parameters, itemCounts);

        if (totalItemsCount > length)
        {
            ReduceItemCounts(length, totalItemsCount, itemCounts, width, height, density);
        }

        EnqueueRequiredItems(itemCounts, parameters, requiredItems);

        int placingSandCount = requiredItems.Count;
        int[] indexes = ShuffleIndexes(length, placingSandCount);

        PlaceItemsOnMap(width, requiredItems, indexes);
    }

    private int CountTotalItems(IEnumerable<Item> placeableItems, WorldItemParameters parameters, Dictionary<Item, int> itemCounts)
    {
        int totalItemsCount = 0;

        foreach (Item item in placeableItems)
        {
            int count = item.GetItemsForPlace(parameters).Count();
            itemCounts[item] = count;
            totalItemsCount += count;
        }

        return totalItemsCount;
    }

    private void ReduceItemCounts(int length, int totalItemsCount, Dictionary<Item, int> itemCounts, int width, int height, int density)
    {
        double reductionFactor = (double)length / totalItemsCount;
        int reducedItemCount = 0;

        foreach ((Item? item, int count) in itemCounts)
        {
            int reducedCount = (int)Math.Floor(count * reductionFactor);
            reducedItemCount += reducedCount;
            itemCounts[item] = reducedCount;
        }

        foreach ((Item? item, int count) in itemCounts)
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

            itemCounts[item]++;
            reducedItemCount++;
        }
    }

    private void EnqueueRequiredItems(Dictionary<Item, int> itemCounts, WorldItemParameters parameters, Queue<WorldItem> requiredItems)
    {
        foreach ((Item? item, int count) in itemCounts)
        {
            foreach (WorldItem worldItem in item.GetItemsForPlace(parameters).Take(count))
            {
                requiredItems.Enqueue(worldItem);
            }
        }
    }

    private int[] ShuffleIndexes(int length, int placingSandCount)
    {
        int[] indexes = Enumerable.Range(1, length).ToArray();

        for (int i = 0; i < placingSandCount - 1; i++)
        {
            int j = seeder.Random.Next(i + 1, length);
            (indexes[i], indexes[j]) = (indexes[j], indexes[i]);
        }

        return indexes;
    }

    private void PlaceItemsOnMap(int width, Queue<WorldItem> requiredItems, int[] indexes)
    {
        int placingSandCount = requiredItems.Count;

        for (int i = 0; i < placingSandCount; i++)
        {
            int index = indexes[i];
            int x = index / width;
            int y = index % width;

            if (requiredItems.TryDequeue(out WorldItem? placeable))
            {
                placeItemAction.Invoke(x, y, placeable);
            }
        }
    }
}
