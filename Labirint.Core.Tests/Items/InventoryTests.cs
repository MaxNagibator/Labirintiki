using Labirint.Core.Items;

namespace Labirint.Core.Tests.Items;

[TestFixture]
public class InventoryTests
{
    private Inventory _inventory = null!;

    [SetUp]
    public void SetUp()
    {
        _inventory = new();
    }

    /// <summary>
    /// Тестирует, что инвентарь набирает свой состав из каталога предметов.
    /// Проверяет, что набор типов в инвентаре совпадает с полным списком предметов игры и имена не повторяются.
    /// </summary>
    [Test]
    public void InventoryHoldsEveryCatalogItemTest()
    {
        var items = _inventory.Stacks.Select(stack => stack.Item).ToList();

        Type[] expectedTypes =
        [
            typeof(Sand),
            typeof(Hammer),
            typeof(Bomb),
            typeof(Oil),
            typeof(WalkThroughWallsBottle),
            typeof(WoolYarn),
        ];

        Assert.Multiple(() =>
        {
            Assert.That(items.Select(item => item.GetType()), Is.EquivalentTo(expectedTypes));
            Assert.That(items.Select(item => item.Name).Distinct().Count(), Is.EqualTo(items.Count));
        });
    }

    /// <summary>
    /// Тестирует, что класс Inventory правильно устанавливает значение по умолчанию для каждого стека предметов.
    /// Проверяет, что количество каждого стека предметов равно его значению по умолчанию.
    /// </summary>
    [Test]
    public void CorrectDefaultItemsCountTest()
    {
        Assert.That(_inventory.Stacks, Is.Not.Empty);

        foreach (var itemStack in _inventory.Stacks)
        {
            Assert.That(itemStack.Count, Is.EqualTo(itemStack.DefaultCount), $"Предмет {itemStack.Item.Name}");
        }
    }
}
