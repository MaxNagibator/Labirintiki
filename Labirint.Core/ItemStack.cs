namespace Labirint.Core;

public class ItemStack(Item item)
{
    public int Count { get; private set; } = item.DefaultCount;
    public int DefaultCount { get; } = item.DefaultCount;
    public int MaxCount { get; } = item.MaxCount;

    public bool IsInfinite { get; } = item.MaxCount > 10_000;

    public Item Item { get; } = item;

    public bool TryAdd(int count)
    {
        if (Count + count > MaxCount)
        {
            return false;
        }

        Count += count;
        return true;
    }

    public void Reset()
    {
        Count = DefaultCount;
    }

    public bool CanUse()
    {
        return Count > 0;
    }

    public bool TryUseItem(Position position, Direction? direction, Labyrinth labyrinth)
    {
        if (TryRemove(1) == false)
        {
            return false;
        }

        Item.Use(position, direction, labyrinth);
        return true;
    }

    private bool TryRemove(int count)
    {
        if (Count < count)
        {
            return false;
        }

        Count -= count;
        return true;
    }

    public override string ToString()
    {
        return $"{Item.Name} ({Count}/{MaxCount})";
    }
}
