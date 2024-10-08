﻿namespace Labirint.Core;

public class Inventory
{
    private static Item[]? _allItems;
    private readonly Dictionary<Item, ItemStack> _items;

    public Inventory()
    {
        _items = GetAllDerivedItems().ToDictionary(item => item, item => new ItemStack(item));
    }

    public event EventHandler? InventoryCleared;

    public event EventHandler<Item>? ItemAdded;
    public event EventHandler<Item>? ItemCantAdded;
    public event EventHandler<Item>? ItemCantUsed;
    public event EventHandler<Item>? ItemUsed;

    public event EventHandler<int>? ScoreIncreased;

    /// <summary>
    ///     Все доступные предметы.
    /// </summary>
    public IEnumerable<Item> AllItems => _items.Keys;

    /// <summary>
    ///     Все стеки предметов в инвентаре.
    /// </summary>
    public IEnumerable<ItemStack> Stacks => _items.Values;

    public bool CanUse(Item item)
    {
        return _items.TryGetValue(item, out ItemStack? stack) && stack.CanUse();
    }

    public void Use(Item item, Position position, Direction? direction, Labyrinth labyrinth)
    {
        if (_items.TryGetValue(item, out ItemStack? stack) == false)
        {
            return;
        }

        if (stack.TryUseItem(position, direction, labyrinth))
        {
            ItemUsed?.Invoke(this, stack.Item);
        }
        else
        {
            ItemCantUsed?.Invoke(this, stack.Item);
        }
    }

    public bool TryAdd(Item item, int count = 1)
    {
        if (_items.TryGetValue(item, out ItemStack? stack) == false)
        {
            return false;
        }

        if (stack.TryAdd(count) == false)
        {
            ItemCantAdded?.Invoke(this, stack.Item);
            return false;
        }

        // TODO Убрать проверку на основе типа
        if (item is ScoreItem scoreItem)
        {
            ScoreIncreased?.Invoke(this, scoreItem.CostPerItem * count);
        }

        ItemAdded?.Invoke(this, item);

        return true;
    }

    /// <summary>
    ///     Сбросить количество всех предметов в инвентаре к значению по умолчанию.
    /// </summary>
    public void Clear()
    {
        foreach (ItemStack stack in Stacks)
        {
            stack.Reset();
        }

        InventoryCleared?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerable<Item> GetAllDerivedItems()
    {
        return _allItems ??= typeof(Item).Assembly
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Item)) && type.IsAbstract == false)
            .Select(type => (Item)Activator.CreateInstance(type)!)
            .OrderByDescending(item => item.ControlSettings != null)
            .ThenByDescending(item => item.MaxCount)
            .ToArray();
    }
}
