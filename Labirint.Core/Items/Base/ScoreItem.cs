namespace Labirint.Core.Items.Base;

public abstract class ScoreItem : Item
{
    public override int DefaultCount => 0;
    public override int MaxCount => int.MaxValue;

    public abstract int CostPerItem { get; }
}
