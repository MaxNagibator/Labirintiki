namespace Labirint.Core.Items;

public abstract class Item
{
    public string Name { get; protected init; }
    public string DisplayName { get; protected init; }

    public virtual string Icon => $"/images/items/{Name}.png";

    public ItemStack Stack { get; protected init; }

    public ControlSettings? ControlSettings { get; protected init; }
    public SoundSettings SoundSettings { get; protected init; }

    public abstract int CalculateCountInMaze(int width, int height, int density);

    protected virtual bool TryPickUp(WorldItem worldItem)
    {
        if (Stack.TryAdd(1) == false)
        {
            return false;
        }

        AfterPickUp(worldItem);
        return true;
    }

    protected virtual void AfterPickUp(WorldItem worldItem)
    {
    }

    public virtual bool TryUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        if (Stack.TryRemove(1) == false)
        {
            return false;
        }

        AfterUse(position, direction, labyrinth);
        return true;
    }

    public virtual void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
    }

    protected virtual void AfterPlace(Position position, Labyrinth labyrinth)
    {
        Stack.InMazeCount++;
    }

    public IEnumerable<WorldItem> GetItemsForPlace(int width, int height, int density)
    {
        int requiredCount = CalculateCountInMaze(width, height, density);

        for (int i = 0; i < requiredCount; i++)
        {
            yield return GetWorldItem();
        }
    }

    public virtual WorldItem GetWorldItem()
    {
        return new WorldItem
        {
            ImageSource = Icon,
            Alignment = Alignment.Center,
            PickUpSound = SoundSettings.PickUpSound,
            Scale = 0.9,
            PickUp = TryPickUp,
            AfterPlace = AfterPlace
        };
    }
}
