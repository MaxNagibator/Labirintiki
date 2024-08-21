namespace Labirint.Core.Stacks;

public class ScoreItemStack(Item item) : ItemStack(item, 0, int.MaxValue)
{
    protected override bool CanAdd(int count)
    {
        return true;
    }
}
