using Labirint.Core.Common;
using Labirint.Core.Interfaces;
using Labirint.Core.Items;
using Labirint.Core.Stacks;
using Labirint.Core.Tests.Helpers;
using DirectionExtensions = Labirint.Core.Tests.Helpers.DirectionExtensions;

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
        _labyrinth.Init(16, 16, 40, _inventory.AllItems);
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

    /// <summary>
    ///     Тестирует, что класс Labyrinth не создает и не разрушает стены с некорректными координатами.
    ///     Проверяет, что вызов методов CreateWall и BreakWall с отрицательными координатами не вызывает исключений.
    /// </summary>
    /// <param name="x">Позиция X клетки</param>
    /// <param name="y">Позиция Y клетки</param>
    [Test]
    [TestCase(0, 0)]
    [TestCase(-1, 0)]
    [TestCase(0, -1)]
    [TestCase(-1, -1)]
    [TestCase(10, 10)]
    [TestCase(-11, 10)]
    [TestCase(10, -11)]
    [TestCase(-11, -11)]
    public void DontCreateOrBreakIncorrectWallsTest(int x, int y)
    {
        foreach (Direction direction in DirectionExtensions.GetAll())
        {
            Assert.DoesNotThrow(() => _labyrinth.CreateWall((x, y), direction));
            Assert.DoesNotThrow(() => _labyrinth.BreakWall((x, y), direction));
        }

        Assert.DoesNotThrow(() => _labyrinth.CreateWall((x, y), Direction.All));
        Assert.DoesNotThrow(() => _labyrinth.BreakWall((x, y), Direction.All));

        Assert.DoesNotThrow(() => _labyrinth.CreateWall((x, y), directions: [Direction.Left, Direction.Top, Direction.Right, Direction.Bottom]));
        Assert.DoesNotThrow(() => _labyrinth.BreakWall((x, y), Direction.Left, Direction.Top, Direction.Right, Direction.Bottom));
    }
}
