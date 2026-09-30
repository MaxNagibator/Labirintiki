using Labirint.Core.Abilities;

namespace Labirint.Core.Tests.Runners;

[TestFixture]
public class WalkThroughWallsAbilityTests : LabyrinthTestsBase
{
    /// <summary>
    /// Тестирует, что WalkThroughWallsAbility деактивируется после исчерпания лимита ходов и дальше движение снова блокируется стеной.
    /// Проверяет, что все пять ходов через стену прошли успешно, после них способность неактивна, а следующий ход в ту же стену блокируется.
    /// </summary>
    [Test]
    public void AbilityStopsIgnoringWallsAfterMoveCountIsExhaustedTest()
    {
        Runner runner = new((5, 5), Labyrinth, new());
        Labyrinth.CreateWall((5, 5), Direction.Right);
        runner.AddAbility(new WalkThroughWallsAbility());

        Direction[] moves = [Direction.Right, Direction.Left, Direction.Right, Direction.Left, Direction.Right];
        var results = moves.Select(runner.Move).ToArray();

        var isActiveAfterMoves = runner.Abilities.Single().Active;
        var isBlockedMove = runner.Move(Direction.Left) == false;

        Assert.Multiple(() =>
        {
            Assert.That(results, Is.All.True);
            Assert.That(isActiveAfterMoves, Is.False);
            Assert.That(isBlockedMove, Is.True);
            Assert.That(runner.Position, Is.EqualTo(new Position(6, 5)));
        });
    }
}
