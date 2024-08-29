namespace Labirint.Core.TileFeatures;

public class WorldItem(Item item, string imageSource, Alignment alignment, double scale)
    : TileFeature
{
    public required Action<Position, Labyrinth> AfterPlace { get; init; }
    public int? PickUpCount { get; init; }

    public override bool RemoveAfterSuccessPickUp => true;
    public override string PickUpSound => item.SoundSettings?.PickUpSound ?? string.Empty;
    public override DrawingSettings? DrawingSettings { get; } = new(imageSource, alignment, scale, 1);

    public override bool TryPickUp(Labyrinth labyrinth)
    {
        return labyrinth.Runner.Inventory.TryAdd(item, PickUpCount ?? 1);
    }
}
