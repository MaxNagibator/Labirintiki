using Labirint.Core.Items;

namespace Labirint.Core.TileFeatures;

public class WorldItem : TileFeature
{
    private Item _item;

    public WorldItem(Item item)
    {
        _item = item;
    }

    public required string ImageSource { get; init; }
    public required Alignment Alignment { get; set; }
    public required double Scale { get; set; }

    public required Func<WorldItem, bool> PickUp { get; init; }
    public required Action<Position, Labyrinth> AfterPlace { get; init; }

    public override bool RemoveAfterSuccessPickUp => true;

    public override string PickUpSound => _item.SoundSettings?.PickUpSound ?? string.Empty;

    public override bool TryPickUp()
    {
        return PickUp.Invoke(this);
    }
}
