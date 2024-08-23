namespace Labirint.Core.TileFeatures;

public abstract class TileFeature
{
    public abstract bool TryPickUp();
    public abstract bool RemoveAfterSuccessPickUp { get; }
    public abstract string PickUpSound { get; }
}
