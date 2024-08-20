using Labirint.Core.Interfaces;
using Labirint.Core.Items;
using Labirint.Core.Stacks;
using Labirint.Core.Tests.Helpers;

namespace Labirint.Core.Tests;

[TestFixture]
public class LabyrinthTests
{
    [SetUp]
    public void SetUp()
    {
        _random = new TestRandom();
        _labyrinth = new Labyrinth(_random);
        _inventory = new Inventory();
    }

    [TearDown]
    public void TearDown()
    {
        _inventory.Clear();
    }

    private IRandom _random;
    private Labyrinth _labyrinth;
    private Inventory _inventory;

    /// <summary>
    ///     Тестирует, что класс Labyrinth размещает правильное количество предметов в лабиринте.
    ///     Проверяет, что количество размещенных предметов соответствует нужному количеству.
    /// </summary>
    /// <param name="width">Ширина лабиринта</param>
    /// <param name="height">Высота лабиринта</param>
    /// <param name="density">Плотность стен в лабиринте</param>
    /// <remarks>Была ошибка, что выдавались только песочки.</remarks>
    [Test]
    [TestCase(16, 16, 40)]
    [TestCase(32, 32, 80)]
    [TestCase(128, 128, 10)]
    public void PlacedCorrectCountOfItemsTest(int width, int height, int density)
    {
        _labyrinth.Init(width, height, density, _inventory.AllItems);

        foreach (ItemStack itemStack in _inventory.Stacks)
        {
            Item item = itemStack.Item;
            int expectedCount = itemStack.Item.CalculateCountInMaze(width, height, density);

            // TODO исправить костыль с определение по пути
            int count = _labyrinth.Enumerate()
                .Where(tile => tile.WorldItem != null)
                .Count(tile => tile.WorldItem!.ImageSource.Contains(item.Name));

            Console.WriteLine($"{item.Name}: {count}/{expectedCount}");
            Assert.That(count, Is.EqualTo(expectedCount));
        }
    }
}
