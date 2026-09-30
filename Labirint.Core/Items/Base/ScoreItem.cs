namespace Labirint.Core.Items.Base;

public abstract class ScoreItem : Item
{
    public override int DefaultCount => 0;
    public override int MaxCount => int.MaxValue;

    public abstract int CostPerItem { get; }

    public override string Kind => "Сокровище";

    public override IReadOnlyList<ItemStat> Stats => [new("Ценность", CostPerItem.ToString(), " очк. за штуку")];

    public override bool TryPickUp(int count, IPickupContext context)
    {
        if (base.TryPickUp(count, context) == false)
        {
            return false;
        }

        context.AddScore((int)Math.Min((long)CostPerItem * count, int.MaxValue));

        return true;
    }
}
