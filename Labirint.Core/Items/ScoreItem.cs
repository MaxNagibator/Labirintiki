namespace Labirint.Core.Items;

public abstract class ScoreItem : Item
{
    protected ScoreItem()
    {
        Stack = new ScoreItemStack(this);
    }

    public int CostPerItem { get; protected init; }
    public int Score => Stack.Count * CostPerItem;
}
