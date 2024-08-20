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

    /// <summary>
    ///     Все доступные предметы.
    /// </summary>
    public IEnumerable<Item> AllItems => _allItems;

    /// <summary>
    ///     Все стеки предметов в инвентаре.
    /// </summary>
    public IEnumerable<ItemStack> Stacks => _stacks ??= GetStacks();

    /// <summary>
    ///     Все ценные предметы в инвентаре, отсортированных по убыванию стоимости.
    /// </summary>
    public IEnumerable<ScoreItem> ScoreItems => _scoreItems ??= GetScoreItems();

    /// <summary>
    ///     Сбросить количество всех предметов в инвентаре к значению по умолчанию.
    /// </summary>
    public void Clear()
    {
        foreach (ItemStack stack in Stacks)
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
