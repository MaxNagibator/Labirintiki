namespace Labirint.Core.TileFeatures;

public class WorldItem : TileFeature
{
    private readonly Item _item;

    public WorldItem(Item item, string imageSource, Alignment alignment, double scale)
    {
        _item = item;

        DrawingSettings = new DrawingSettings(imageSource, alignment, scale);
    }

    public required Func<WorldItem, bool> PickUp { get; init; }
    public required Action<Position, Labyrinth> AfterPlace { get; init; }

    public override bool RemoveAfterSuccessPickUp => true;

    public override string PickUpSound => _item.SoundSettings?.PickUpSound ?? string.Empty;
    public override DrawingSettings? DrawingSettings { get; }

    public override bool TryPickUp()
    {
        return PickUp.Invoke(this);
    }
}
