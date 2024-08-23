namespace Labirint.Core.Stacks.Base;

public abstract class ItemStack(Item item, int defaultCount, int maxCount)
{
    public int Count { get; private set; } = defaultCount;
    public int DefaultCount { get; } = defaultCount;
    public int MaxCount { get; } = maxCount;
    public int InMazeCount { get; set; }
    public Item Item { get; } = item;

    public bool IsInfinite { get; } = maxCount > 10_000;

    public bool TryRemove(int count)
    {
        if (Count < count)
        {
            return false;
        }

        Count -= count;
        return true;
    }

    public bool TryAdd(int count)
    {
        if (CanAdd(count) == false)
        {
            return false;
        }

        Count += count;
        return true;
    }

    public void Reset()
    {
        Count = DefaultCount;
        InMazeCount = 0;
    }

    protected abstract bool CanAdd(int count);

    public override string ToString()
    {
        return $"{Item.Name} ({InMazeCount})";
    }
}
