using Labirint.Core.TileFeatures;

namespace Labirint.Core.Items.Base;

public abstract class Item
{
    public abstract string Name { get; }
    public abstract string DisplayName { get; }
    public abstract string Description { get; }

    public abstract int DefaultCount { get; }
    public abstract int MaxCount { get; }

    /// <summary>
    /// Используется сразу после подбора.
    /// </summary>
    public virtual bool UseAfterPickup => false;

    public string Icon => $"images/items/{Name}-icon.png";
    public string Image => $"images/items/{Name}.png";

    public virtual ControlSettings? ControlSettings => null;
    public virtual SoundSettings? SoundSettings => null;

    public virtual WorldItem GetWorldItem(WorldItemParameters parameters)
    {
        return new WorldItem(this, Image, Alignment.Center, 0.9)
        {
            AfterPlace = AfterPlace
        };
    }

    public void Use(Position position, Direction? direction, Labyrinth labyrinth)
    {
        AfterUse(position, direction, labyrinth);
    }

    public abstract int CalculateCountInMaze(int width, int height, int density);

    public IEnumerable<WorldItem> GetItemsForPlace(WorldItemParameters parameters)
    {
        int requiredCount = CalculateCountInMaze(parameters.Width, parameters.Height, parameters.Density);

        for (int i = 0; i < requiredCount; i++)
        {
            yield return GetWorldItem(parameters);
        }
    }

    protected virtual void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
    }

    protected virtual void AfterPlace(Position position, Labyrinth labyrinth)
    {
    }
}
