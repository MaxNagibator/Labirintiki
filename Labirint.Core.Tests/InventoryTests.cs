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

    /// <summary>
    ///     Тестирует, что класс Inventory правильно загружает доступные предметы.
    ///     Проверяет, что список загруженных предметов не пуст.
    /// </summary>
    [Test]
    public void CorrectLoadItemsTest()
    {
        List<string> inventoryAvailableItems = _inventory.Stacks.Select(x => x.Item.Name).ToList();

        Console.WriteLine($"Загруженные предметы: {string.Join(", ", inventoryAvailableItems)}");

        Assert.That(inventoryAvailableItems, Is.Not.Empty);
    }

    /// <summary>
    ///     Тестирует, что класс Inventory правильно загружает оцениваемые предметы.
    ///     Проверяет, что список загруженных оцениваемых предметов не пуст.
    /// </summary>
    [Test]
    public void CorrectLoadScoredItemsTest()
    {
        List<string> inventoryAvailableItems = _inventory.ScoreItems
            .Select(scoreItem => $"{scoreItem.Name} ({scoreItem.CostPerItem})")
            .ToList();

        Console.WriteLine($"Загруженные предметы: {string.Join(", ", inventoryAvailableItems)}");

        Assert.That(inventoryAvailableItems, Is.Not.Empty);
    }

    /// <summary>
    ///     Тестирует, что класс Inventory правильно устанавливает значение по умолчанию для каждого стека предметов.
    ///     Проверяет, что количество каждого стека предметов равно его значению по умолчанию.
    /// </summary>
    [Test]
    public void CorrectDefaultItemsCountTest()
    {
        foreach (ItemStack itemStack in _inventory.Stacks)
        {
            Assert.That(itemStack.Count, Is.EqualTo(itemStack.DefaultCount));
        }
    }
}
