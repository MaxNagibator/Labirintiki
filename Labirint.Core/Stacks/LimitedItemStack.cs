namespace Labirint.Core.Stacks;

public class LimitedItemStack(Item item, int defaultCount, int maxCount) : ItemStack(item, defaultCount, maxCount)
{
    public override bool TryAdd(int count)
    {
        if (Count + count > MaxCount)
        {
            return false;
        }

        Count += count;
        return true;
    }
}
