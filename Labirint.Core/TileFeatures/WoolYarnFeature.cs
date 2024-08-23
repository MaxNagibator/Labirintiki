namespace Labirint.Core.TileFeatures;

public class WoolYarnFeature : TileFeature
{
    public override bool RemoveAfterSuccessPickUp => false;

    public override bool TryPickUp()
    {
        return false;
    }
}
