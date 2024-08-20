namespace Labirint.Core.Stacks;

public class ScoreItemStack(Item item) : ItemStack(item, 0, int.MaxValue)
{
    public override bool TryAdd(int count)
    {
        Count += count;
        return true;
    }
}
