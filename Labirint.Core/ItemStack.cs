namespace Labirint.Core;

public class ItemStack
{
    public ItemStack(Item item)
    {
        Item = item;
        Item.InitStack(this);

        Count = Item.DefaultCount;
        MaxCount = Item.MaxCount;
    }

    public int Count { get; private set; }
    public int MaxCount { get; }

    public Item Item { get; }

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
        if (Count + count > MaxCount)
        {
            return false;
        }

        Count += count;
        return true;
    }

    public void Reset()
    {
        Count = Item.DefaultCount;
    }
}
