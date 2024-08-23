using System.Collections.ObjectModel;
using Labirint.Core.Stacks.Base;

namespace LabirintBlazorApp.Services;

public class InventoryService
{
    private readonly Inventory _inventory;

    public InventoryService()
    {
        _inventory = new Inventory();
        Stacks = new ObservableCollection<ItemStack>(_inventory.Stacks);
        ScoreItems = new ObservableCollection<ScoreItem>(_inventory.ScoreItems);
    }

    /// <inheritdoc cref="Inventory.AllItems" />
    public IEnumerable<Item> Items => _inventory.AllItems;

    /// <inheritdoc cref="Inventory.Stacks" />
    public ObservableCollection<ItemStack> Stacks { get; }

    /// <inheritdoc cref="Inventory.ScoreItems" />
    public ObservableCollection<ScoreItem> ScoreItems { get; }

    /// <inheritdoc cref="Inventory.Clear" />
    public void Clear()
    {
        _inventory.Clear();
    }
}
