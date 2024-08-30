namespace Labirint.Core.TileFeatures;

public class WoolYarnFeature(Direction direction) : TileFeature
{
    public override bool RemoveAfterSuccessPickUp => false;
    public override string PickUpSound => string.Empty;
    public override DrawingSettings DrawingSettings { get; } = new($"images/features/wool-yarn-{direction}.png", Alignment.Stretch, 1);

    public override bool TryPickUp(Labyrinth labyrinth)
    {
        return false;
    }
}
