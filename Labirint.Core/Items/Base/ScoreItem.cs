namespace Labirint.Core.Items.Base;

public abstract class ScoreItem : Item
{
    protected ScoreItem()
    {
        Stack = new ScoreItemStack(this);
    }

    public int CostPerItem { get; protected init; }
    public int Score => Stack.Count * CostPerItem;
}
