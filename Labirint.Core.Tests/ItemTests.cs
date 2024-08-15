using Labirint.Core.Items;

namespace Labirint.Core.Tests;

[TestFixture]
public class ItemTests
{
    [Test]
    [Ignore("Для проверки калькуляции. Зависит от формулы и требует ручных корректировок")]
    [TestCase(typeof(Sand), 16)]
    [TestCase(typeof(Hammer), 3)]
    [TestCase(typeof(Bomb), 1)]
    public void ItemsCorrectCalculateCountInMazeTest(Type itemType, int expectedCount)
    {
        int width = 16;
        int height = 16;
        int density = 40;

        Item item = (Item)Activator.CreateInstance(itemType)!;
        int count = item.CalculateCountInMaze(width, height, density);

        Assert.That(count, Is.EqualTo(expectedCount));
    }
}