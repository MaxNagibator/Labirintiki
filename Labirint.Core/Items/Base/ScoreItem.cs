namespace Labirint.Core.Items.Base;

public abstract class ScoreItem : Item
{
    protected ScoreItem()
    {
        Stack = new ScoreItemStack(this);
    }

    public abstract int CostPerItem { get;  }
    public int Score => Stack.Count * CostPerItem;
}
