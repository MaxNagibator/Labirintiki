namespace Labirint.Core.TileFeatures.Base;

public abstract class TileFeature
{
    public abstract bool RemoveAfterSuccessPickUp { get; }
    public abstract string PickUpSound { get; }
    public abstract DrawingSettings? DrawingSettings { get; }
    public abstract bool TryPickUp(Labyrinth labyrinth);
}
