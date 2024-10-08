using DirectionExtensions = Labirint.Core.Tests.Helpers.DirectionExtensions;

namespace Labirint.Core.Tests;

internal class TestItem(int count) : Item
{
    private static int _id;

    public override string Name { get; } = "test " + _id++;
    public override string DisplayName { get; } = "Test " + count;
    public override string Description => string.Empty;

    public override int DefaultCount => 0;
    public override int MaxCount => 0;

    public int Count { get; } = count;

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return Count;
    }
}

[TestFixture]
public class LabyrinthTests : LabyrinthTestsBase
{
    /// <summary>
    ///     Тестирует правильное распределение остатков предметов в лабиринте.
    ///     Проверяет, что количество размещенных предметов соответствует ожидаемому количеству,
    ///     учитывая ограничения по ширине и высоте лабиринта.
    /// </summary>
    /// <param name="width">Ширина лабиринта</param>
    /// <param name="height">Высота лабиринта</param>
    /// <param name="counts">Массив количеств предметов для распределения</param>
    [Test]
    [TestCase(2, 2, 1, 2)]
    [TestCase(2, 2, 2, 2)]
    [TestCase(2, 2, 1, 1, 1, 1)]
    [TestCase(2, 2, 10, 1, 1, 1)]
    [TestCase(2, 2, 2, 0, 1, 1)]
    public void DistributionOfRemainderTest(int width, int height, params int[] counts)
    {
        int allCount = counts.Sum();
        int placedCount = 0;

        List<TestItem> items = counts.Select(x => new TestItem(x)).ToList();
        Labyrinth.Init(width, height, 40, items);

        foreach (TestItem item in items)
        {
            int expectedCount = Math.Min(item.Count, Math.Min(placedCount + item.Count, width * height - 1 - placedCount));

            int count = Labyrinth.GetInMazeCount(item);

            placedCount += count;

            Console.WriteLine($"{item.Name}({item.Count}): {count}/{expectedCount}");
            Assert.That(count, Is.EqualTo(expectedCount));
        }

        Console.WriteLine($"Всего: {placedCount}/{allCount}");
    }

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
        Labyrinth.Init(width, height, density, Inventory.AllItems);

        foreach (ItemStack itemStack in Inventory.Stacks)
        {
            Item item = itemStack.Item;
            int expectedCount = itemStack.Item.CalculateCountInMaze(width, height, density);

            int count = Labyrinth.GetInMazeCount(item);

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
            Assert.DoesNotThrow(() => Labyrinth.CreateWall((x, y), direction));
            Assert.DoesNotThrow(() => Labyrinth.BreakWall((x, y), direction));
        }

        Assert.DoesNotThrow(() => Labyrinth.CreateWall((x, y), Direction.All));
        Assert.DoesNotThrow(() => Labyrinth.BreakWall((x, y), Direction.All));

        Assert.DoesNotThrow(() => Labyrinth.CreateWall((x, y), directions: [Direction.Left, Direction.Top, Direction.Right, Direction.Bottom]));
        Assert.DoesNotThrow(() => Labyrinth.BreakWall((x, y), Direction.Left, Direction.Top, Direction.Right, Direction.Bottom));
    }

    /// <summary>
    ///     Тестирует, что класс Labyrinth корректно определяет, является ли позиция корректной внутри лабиринта.
    ///     Проверяет, что метод IsCorrectPosition возвращает ожидаемый результат для различных координат.
    /// </summary>
    /// <param name="x">Позиция X клетки</param>
    /// <param name="y">Позиция Y клетки</param>
    /// <param name="expectedResult">Ожидаемый результат проверки корректности позиции</param>
    [TestCase(5, 5, true)]
    [TestCase(0, 0, true)]
    [TestCase(DefaultWidth, DefaultHeight, false)]
    [TestCase(DefaultWidth + 1, DefaultHeight + 1, false)]
    [TestCase(-1, -1, false)]
    [TestCase(9, 9, true)]
    [TestCase(0, 9, true)]
    [TestCase(9, 0, true)]
    [TestCase(DefaultWidth + 1, 0, false)]
    [TestCase(0, DefaultHeight, false)]
    [TestCase(5, DefaultHeight + 1, false)]
    [TestCase(DefaultWidth + 1, 5, false)]
    [TestCase(-1, 5, false)]
    [TestCase(5, -1, false)]
    public void IsCorrectPositionTest(int x, int y, bool expectedResult)
    {
        Position position = (x, y);

        bool result = Labyrinth.IsCorrectPosition(position);

        Assert.That(result, Is.EqualTo(expectedResult));
    }
}
