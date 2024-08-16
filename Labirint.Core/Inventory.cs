namespace Labirint.Core;

public class Inventory
{
    private readonly List<Item> _allItems;

    private List<ScoreItem>? _scoreItems;
    private List<ItemStack>? _stacks;

    public Inventory()
    {
        _allItems = GetAllDerivedItems().ToList();
    }

    public IEnumerable<ItemStack> Items => _stacks ??= GetStacks();

    public IEnumerable<ScoreItem> ScoreItems => _scoreItems ??= GetScoreItems();

    public void Clear()
    {
        foreach (ItemStack stack in Items)
        {
            stack.Reset();
        }
    }

    private List<ItemStack> GetStacks()
    {
        return _allItems
            .Select(item => item.Stack)
            .ToList();
    }

    private List<ScoreItem> GetScoreItems()
    {
        return _allItems
            .Where(item => item.GetType().IsSubclassOf(typeof(ScoreItem)))
            .Select(item => (ScoreItem)item)
            .OrderByDescending(item => item.CostPerItem)
            .ToList();
    }

    private IEnumerable<Item> GetAllDerivedItems()
    {
        return typeof(Item).Assembly
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Item)) && type.IsAbstract == false)
            .Select(type => (Item)Activator.CreateInstance(type)!)
            .OrderByDescending(item => item.Stack.MaxCount);
    }
}
