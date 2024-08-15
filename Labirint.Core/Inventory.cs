namespace Labirint.Core;

public class Inventory
{
    private readonly List<Item> _items;
    private List<ItemStack>? _stacks;

    public Inventory()
    {
        _items = GetAllDerivedItems().ToList();
    }

    public IEnumerable<ItemStack> Items => _stacks ??= _items
        .Select(x => new ItemStack(x))
        .ToList();

    public void Clear()
    {
        foreach (ItemStack stack in Items)
        {
            stack.Reset();
        }
    }

    private IEnumerable<Item> GetAllDerivedItems()
    {
        return typeof(Item).Assembly
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Item)) && type.IsAbstract == false)
            .Select(type => (Item)Activator.CreateInstance(type)!)
            .OrderByDescending(item => item.MaxCount);
    }
}
