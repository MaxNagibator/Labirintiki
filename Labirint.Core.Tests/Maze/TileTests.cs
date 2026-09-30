namespace Labirint.Core.Tests.Maze;

[TestFixture]
public class TileTests
{
    private const Direction ThreeWalls = Direction.Left | Direction.Top | Direction.Right;

    /// <summary>
    /// Тестирует, что клетка разрешает добавить стену только тогда, когда та не замурует клетку со всех сторон.
    /// Проверяет, что метод CanAddWall возвращает ожидаемый результат для набора стен клетки.
    /// </summary>
    /// <param name="existingWalls">Стены, уже стоящие в клетке</param>
    /// <param name="directionToAdd">Направление добавляемой стены</param>
    /// <param name="expectedResult">Ожидаемый результат проверки</param>
    [TestCase(Direction.None, Direction.None, false)]
    [TestCase(Direction.Left, Direction.None, false)]
    [TestCase(Direction.All, Direction.None, false)]
    [TestCase(Direction.None, Direction.Left, true)]
    [TestCase(Direction.Left, Direction.Top, true)]
    [TestCase(Direction.Left | Direction.Top, Direction.Right, true)]
    [TestCase(Direction.Left, Direction.Left, false)]
    [TestCase(Direction.Left | Direction.Top, Direction.Left, false)]
    [TestCase(Direction.Left | Direction.Bottom, Direction.Top, true)]
    [TestCase(Direction.Top | Direction.Right | Direction.Bottom, Direction.Left, false)]
    [TestCase(ThreeWalls, Direction.Left, false)]
    [TestCase(ThreeWalls, Direction.Bottom, false)]
    [TestCase(Direction.All, Direction.Left, false)]
    [TestCase(Direction.None, Direction.All, false)]
    public void CanAddWallTest(Direction existingWalls, Direction directionToAdd, bool expectedResult)
    {
        Labyrinth labyrinth = new(new SeedRandom(1));

        Tile tile = new(labyrinth)
        {
            Walls = existingWalls,
        };

        var result = tile.CanAddWall(directionToAdd);

        Assert.That(result, Is.EqualTo(expectedResult));
    }
}
