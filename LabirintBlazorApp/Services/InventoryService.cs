using System.Collections.ObjectModel;

namespace LabirintBlazorApp.Services;

public class InventoryService
{
    private readonly Inventory _inventory;

    public InventoryService()
    {
        _inventory = new Inventory();
        Stacks = new ObservableCollection<ItemStack>(_inventory.Items);
    }

    /// <inheritdoc cref="Inventory.Items" />
    public IEnumerable<Item> Items => _inventory.Items.Select(x => x.Item);

    public ObservableCollection<ItemStack> Stacks { get; }

    public void Clear()
    {
        _inventory.Clear();
    }
}
