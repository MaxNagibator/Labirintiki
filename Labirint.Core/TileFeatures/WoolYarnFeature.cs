namespace Labirint.Core.TileFeatures;

public class WoolYarnFeature : TileFeature
{
    public override bool RemoveAfterSuccessPickUp => false;
    public override string PickUpSound => string.Empty;
    public override DrawingSettings? DrawingSettings => null;

    public override bool TryPickUp()
    {
        return false;
    }
}
