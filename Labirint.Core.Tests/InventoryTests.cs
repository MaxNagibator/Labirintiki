namespace Labirint.Core.Tests;

[TestFixture]
public class InventoryTests
{
    [SetUp]
    public void SetUp()
    {
        _inventory = new Inventory();
    }

    private Inventory _inventory;

    [Test]
    public void CorrectLoadItemsTest()
    {
        List<string> inventoryAvailableItems = _inventory.Items.Select(x => x.Item.Name).ToList();

        Console.WriteLine($"Загруженные предметы: {string.Join(", ", inventoryAvailableItems)}");

        Assert.That(inventoryAvailableItems, Is.Not.Empty);
    }   
    
    [Test]
    public void CorrectDefaultItemsCountTest()
    {
        foreach (ItemStack itemStack in _inventory.Items)
        {
            Assert.That(itemStack.Count, Is.EqualTo(itemStack.Item.DefaultCount));
        }
    }
}
