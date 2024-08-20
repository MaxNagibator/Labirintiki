using Labirint.Core.Items;

namespace Labirint.Core.Tests;

[TestFixture]
public class ItemTests
{
    // TODO убрать дублирование параметров лабиринта для каждого предмета
    /// <summary>
    ///     Тестирует, что метод расчета количества предметов правильно рассчитывает количество предметов в лабиринте.
    ///     Проверяет, что расчетное количество равно ожидаемому.
    /// </summary>
    /// <param name="itemType">Тип предмета</param>
    /// <param name="width">Ширина лабиринта</param>
    /// <param name="height">Высота лабиринта</param>
    /// <param name="density">Плотность стен в лабиринте</param>
    /// <param name="expectedCount">Ожидаемое количество предметов</param>
    [Test]
    [TestCase(typeof(Sand), 16, 16, 40, 16)]
    [TestCase(typeof(Hammer), 16, 16, 40, 3)]
    [TestCase(typeof(Bomb), 16, 16, 40, 1)]
    [TestCase(typeof(Oil), 16, 16, 40, 0)]
    [TestCase(typeof(Sand), 32, 32, 20, 32)]
    [TestCase(typeof(Hammer), 32, 32, 20, 3)]
    [TestCase(typeof(Bomb), 32, 32, 20, 1)]
    [TestCase(typeof(Oil), 32, 32, 20, 1)]
    public void ItemsCorrectCalculateCountInMazeTest(Type itemType, int width, int height, int density, int expectedCount)
    {
        Item item = (Item)Activator.CreateInstance(itemType)!;

        int count = item.CalculateCountInMaze(width, height, density);

        Assert.That(count, Is.EqualTo(expectedCount));
    }
}
