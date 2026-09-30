using Labirint.Core.Extensions;
using DirectionExtensions = Labirint.Core.Tests.Helpers.DirectionExtensions;

namespace Labirint.Core.Tests.Maze;

[TestFixture]
public class LabyrinthTests : LabyrinthTestsBase
{
    /// <summary>
    /// Тестирует правильное распределение остатков предметов в лабиринте.
    /// Проверяет, что количество размещенных предметов соответствует ожидаемому количеству,
    /// учитывая ограничения по ширине и высоте лабиринта.
    /// </summary>
    /// <param name="width">Ширина лабиринта</param>
    /// <param name="height">Высота лабиринта</param>
    /// <param name="counts">Массив количеств предметов для распределения</param>
    [TestCase(2, 2, 1, 2)]
    [TestCase(2, 2, 2, 2)]
    [TestCase(2, 2, 1, 1, 1, 1)]
    [TestCase(2, 2, 10, 1, 1, 1)]
    [TestCase(2, 2, 2, 0, 1, 1)]
    public void DistributionOfRemainderTest(int width, int height, params int[] counts)
    {
        var placedCount = 0;

        var items = counts.Select(x => new TestItem(x)).ToList();
        Labyrinth.Init(width, height, 40, items);

        foreach (var item in items)
        {
            var expectedCount = Math.Min(item.Count, Math.Min(placedCount + item.Count, width * height - 1 - placedCount));

            var count = Labyrinth.GetInMazeCount(item);

            placedCount += count;

            Assert.That(count, Is.EqualTo(expectedCount), $"Предмет {item.Name} на {item.Count} штук");
        }
    }

    /// <summary>
    /// Тестирует, что класс Labyrinth размещает правильное количество предметов в лабиринте.
    /// Проверяет, что количество размещенных предметов соответствует нужному количеству.
    /// </summary>
    /// <param name="width">Ширина лабиринта</param>
    /// <param name="height">Высота лабиринта</param>
    /// <param name="density">Плотность стен в лабиринте</param>
    /// <remarks>Была ошибка, что выдавались только песочки.</remarks>
    [TestCase(16, 16, 40)]
    [TestCase(32, 32, 80)]
    [TestCase(128, 128, 10)]
    public void PlacedCorrectCountOfItemsTest(int width, int height, int density)
    {
        Labyrinth.Init(width, height, density, Inventory.AllItems);

        foreach (var itemStack in Inventory.Stacks)
        {
            var item = itemStack.Item;
            var expectedCount = itemStack.Item.CalculateCountInMaze(width, height, density);

            var count = Labyrinth.GetInMazeCount(item);

            Assert.That(count, Is.EqualTo(expectedCount), $"Предмет {item.Name}");
        }
    }

    /// <summary>
    /// Тестирует, что класс Labyrinth не создает и не разрушает стены с некорректными координатами.
    /// Проверяет, что сетка стен не изменилась после каждого вызова CreateWall и BreakWall по отдельности.
    /// </summary>
    /// <param name="x">Позиция X клетки</param>
    /// <param name="y">Позиция Y клетки</param>
    [TestCase(-1, 0)]
    [TestCase(0, -1)]
    [TestCase(-1, -1)]
    [TestCase(-11, 10)]
    [TestCase(10, -11)]
    [TestCase(-11, -11)]
    [TestCase(DefaultWidth, 0)]
    [TestCase(0, DefaultHeight)]
    public void IncorrectPositionsLeaveWallsUntouchedTest(int x, int y)
    {
        var expected = SnapshotWalls();

        List<Action> calls = [];

        foreach (var direction in DirectionExtensions.GetAll())
        {
            calls.Add(() => Labyrinth.CreateWall((x, y), direction));
            calls.Add(() => Labyrinth.BreakWall((x, y), direction));
        }

        calls.Add(() => Labyrinth.CreateWall((x, y), Direction.All));
        calls.Add(() => Labyrinth.BreakWall((x, y), Direction.All));
        calls.Add(() => Labyrinth.CreateWall((x, y), directions: [Direction.Left, Direction.Top, Direction.Right, Direction.Bottom]));
        calls.Add(() => Labyrinth.BreakWall((x, y), Direction.Left, Direction.Top, Direction.Right, Direction.Bottom));

        Assert.Multiple(() =>
        {
            for (var index = 0; index < calls.Count; index++)
            {
                calls[index].Invoke();

                Assert.That(SnapshotWalls(), Is.EqualTo(expected), $"Вызов {index} изменил сетку стен");
            }
        });
    }

    /// <summary>
    /// Тестирует, что класс Labyrinth создает и разрушает стену с корректными координатами.
    /// Проверяет, что стена появляется в клетке и зеркалится в соседнюю, а затем исчезает из обеих.
    /// </summary>
    /// <param name="x">Позиция X клетки</param>
    /// <param name="y">Позиция Y клетки</param>
    /// <param name="direction">Направление стены</param>
    [TestCase(0, 0, Direction.Right)]
    [TestCase(10, 10, Direction.Top)]
    [TestCase(DefaultWidth - 1, DefaultHeight - 1, Direction.Left)]
    public void CorrectPositionWallIsCreatedAndBrokenTest(int x, int y, Direction direction)
    {
        Position position = (x, y);
        var adjacent = direction.GetAdjacentPosition(position);
        var opposite = direction.GetOppositeDirection();

        Labyrinth[position].Walls = Direction.None;
        Labyrinth[adjacent].Walls = Direction.None;

        Labyrinth.CreateWall(position, direction);
        var isCreated = Labyrinth[position].ContainsWall(direction);
        var isMirrored = Labyrinth[adjacent].ContainsWall(opposite);

        Labyrinth.BreakWall(position, direction);
        var isBroken = Labyrinth[position].ContainsWall(direction) == false;
        var isMirrorBroken = Labyrinth[adjacent].ContainsWall(opposite) == false;

        Assert.Multiple(() =>
        {
            Assert.That(isCreated, Is.True);
            Assert.That(isMirrored, Is.True);
            Assert.That(isBroken, Is.True);
            Assert.That(isMirrorBroken, Is.True);
        });
    }

    /// <summary>
    /// Тестирует, что класс Labyrinth корректно определяет, является ли позиция корректной внутри лабиринта.
    /// Проверяет, что метод IsCorrectPosition возвращает ожидаемый результат для различных координат.
    /// </summary>
    /// <param name="x">Позиция X клетки</param>
    /// <param name="y">Позиция Y клетки</param>
    /// <param name="expectedResult">Ожидаемый результат проверки корректности позиции</param>
    [TestCase(0, 0, true)]
    [TestCase(DefaultWidth - 1, DefaultHeight - 1, true)]
    [TestCase(DefaultWidth, DefaultHeight, false)]
    [TestCase(DefaultWidth, 0, false)]
    [TestCase(0, DefaultHeight, false)]
    [TestCase(-1, 0, false)]
    [TestCase(0, -1, false)]
    public void IsCorrectPositionTest(int x, int y, bool expectedResult)
    {
        Position position = (x, y);

        var result = Labyrinth.IsCorrectPosition(position);

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    private Direction[] SnapshotWalls()
    {
        return Labyrinth.Enumerate().Select(tile => tile.Walls).ToArray();
    }
}
