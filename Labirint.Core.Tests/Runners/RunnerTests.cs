using Labirint.Core.Abilities;

namespace Labirint.Core.Tests.Runners;

[TestFixture]
public class RunnerTests : LabyrinthTestsBase
{
    /// <summary>
    /// Тестирует, что Runner с активной WalkThroughWallsAbility всё равно останавливается на границе лабиринта.
    /// Проверяет, что Move возвращает false и позиция бегуна не меняется при попытке шагнуть за пределы поля.
    /// </summary>
    /// <param name="x">Начальная позиция X бегуна</param>
    /// <param name="y">Начальная позиция Y бегуна</param>
    /// <param name="direction">Направление движения</param>
    [TestCase(0, 0, Direction.Left)]
    [TestCase(0, 0, Direction.Top)]
    [TestCase(DefaultWidth - 1, DefaultHeight - 1, Direction.Right)]
    [TestCase(DefaultWidth - 1, DefaultHeight - 1, Direction.Bottom)]
    public void WallsIgnoringRunnerStopsAtBorderTest(int x, int y, Direction direction)
    {
        Runner runner = new((x, y), Labyrinth, new());
        runner.AddAbility(new WalkThroughWallsAbility());

        var isMoved = runner.Move(direction);

        Assert.Multiple(() =>
        {
            Assert.That(isMoved, Is.False);
            Assert.That(runner.Position, Is.EqualTo(new Position(x, y)));
        });
    }

    /// <summary>
    /// Тестирует, что Runner останавливается на клетке выхода, у которой намеренно снята нижняя стена.
    /// Проверяет, что нижняя стена отсутствует, при этом Move в клетку за пределами сетки возвращает false и позиция бегуна не меняется.
    /// </summary>
    [Test]
    public void RunnerStopsAtOpenExitTest()
    {
        Position exit = new(DefaultWidth / 2, DefaultHeight - 1);
        Runner runner = new(exit, Labyrinth, new());

        var hasBottomWall = Labyrinth[exit].ContainsWall(Direction.Bottom);
        var isMoved = runner.Move(Direction.Bottom);

        Assert.Multiple(() =>
        {
            Assert.That(hasBottomWall, Is.False);
            Assert.That(isMoved, Is.False);
            Assert.That(runner.Position, Is.EqualTo(exit));
        });
    }

    /// <summary>
    /// Тестирует, что Runner с активной WalkThroughWallsAbility проходит сквозь внутреннюю стену лабиринта.
    /// Проверяет, что стена на клетке действительно установлена, Move возвращает true и бегун перемещается на соседнюю клетку за стеной.
    /// </summary>
    [Test]
    public void WallsIgnoringRunnerCrossesInnerWallTest()
    {
        Runner runner = new((1, 1), Labyrinth, new());
        Labyrinth.CreateWall((1, 1), Direction.Right);
        runner.AddAbility(new WalkThroughWallsAbility());

        var hasRightWall = Labyrinth[1, 1].ContainsWall(Direction.Right);
        var isMoved = runner.Move(Direction.Right);

        Assert.Multiple(() =>
        {
            Assert.That(hasRightWall, Is.True);
            Assert.That(isMoved, Is.True);
            Assert.That(runner.Position, Is.EqualTo(new Position(2, 1)));
        });
    }
}
