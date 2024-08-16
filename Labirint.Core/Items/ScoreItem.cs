namespace Labirint.Core.Items;

public abstract class ScoreItem : Item
{
    public int CostPerItem { get; protected init; }
    public int Score => (_stack?.Count ?? 0) * CostPerItem;
}
