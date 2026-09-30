namespace Labirint.Items.Flame.Tests;

[TestFixture]
public class ItemCatalogSourceTests
{
    /// <summary>
    /// Тестирует, что предмет из отдельной сборки попадает в игру после регистрации этой сборки в каталоге.
    /// Проверяет, что огнемёт появляется и в каталоге предметов, и в свежесозданном инвентаре со своим стеком.
    /// </summary>
    [Test]
    public void RegisteredAssemblyAddsItsItemsTest()
    {
        ItemCatalog.AddSource(typeof(Flamethrower).Assembly);

        Inventory inventory = new();

        Assert.Multiple(() =>
        {
            Assert.That(ItemCatalog.Items.OfType<Flamethrower>().Count(), Is.EqualTo(1));
            Assert.That(inventory.Stacks.Select(stack => stack.Item), Has.Exactly(1).InstanceOf<Flamethrower>());
        });
    }
}
