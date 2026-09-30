namespace Labirint.Core;

internal sealed class PickupContext(Labyrinth labyrinth) : IPickupContext
{
    public bool TryStore(Item item, int count)
    {
        return labyrinth.Runner.Inventory.TryAdd(item, count);
    }

    public void AddScore(int amount)
    {
        labyrinth.Runner.AddScore(amount);
    }

    public void Use(Item item)
    {
        item.Use(labyrinth.Runner.Position, labyrinth.Runner.LastDirection, labyrinth);
    }
}
