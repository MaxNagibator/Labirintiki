namespace Labirint.Core.Stacks;

public class LimitedItemStack(Item item, int defaultCount, int maxCount) : ItemStack(item, defaultCount, maxCount)
{
    protected override bool CanAdd(int count)
    {
        return Count + count <= MaxCount;
    }
}
