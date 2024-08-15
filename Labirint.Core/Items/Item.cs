namespace Labirint.Core.Items;

public abstract class Item
{
    protected ItemStack _stack;

    public string Name { get; init; }
    public string DisplayName { get; init; }

    public string Icon => $"/images/items/{Name}.png";

    // TODO вынести в стэк
    public int DefaultCount { get; set; }
    public int MaxCount { get; init; }

    public ControlSettings? ControlSettings { get; set; }
    public SoundSettings SoundSettings { get; init; }

    public abstract int CalculateCountInMaze(int width, int height, int density);

    public void InitStack(ItemStack itemStack)
    {
        _stack = itemStack;
    }

    public virtual bool TryPickUp(WorldItem worldItem)
    {
        return _stack.TryAdd(1);
    }

    public virtual bool TryUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        return _stack.TryRemove(1);
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
            Scale = 1,
            PickUp = TryPickUp
        };
    }
}
