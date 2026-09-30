using Labirint.Core.Items;
using Labirint.Core.TileFeatures;

namespace Labirint.Core.Tests.Maze;

[TestFixture]
public class LabyrinthEventsTests : LabyrinthTestsBase
{
    /// <summary>
    /// Тестирует, что событие RunnerMoved поднимается до подбора предмета в клетке.
    /// Проверяет, что кадр, снятый по перемещению, еще содержит лежащий в клетке предмет.
    /// </summary>
    [Test]
    public void RunnerMovedIsRaisedBeforeItemIsPickedUpTest()
    {
        var sand = Labyrinth.Runner.Inventory.AllItems.OfType<Sand>().Single();
        var target = new Position(1, 0);

        Labyrinth.BreakWall((0, 0), Direction.Right);
        Labyrinth[target].Features?.Clear();
        Labyrinth[target].AddFeature(new WorldItem(sand, sand.Image, Alignment.Center, 1));

        List<string> raised = [];
        var isItemOnTileWhenMoved = false;

        Labyrinth.RunnerMoved += (_, _) =>
        {
            raised.Add(nameof(Labyrinth.RunnerMoved));
            isItemOnTileWhenMoved = Labyrinth[target].Features?.OfType<WorldItem>().Any() ?? false;
        };

        Labyrinth.ItemPickedUp += (_, _) => raised.Add(nameof(Labyrinth.ItemPickedUp));

        Labyrinth.Move(Direction.Right);

        Assert.Multiple(() =>
        {
            Assert.That(raised, Is.EqualTo(new[] { nameof(Labyrinth.RunnerMoved), nameof(Labyrinth.ItemPickedUp) }));
            Assert.That(isItemOnTileWhenMoved, Is.True);
            Assert.That(Labyrinth[target].Features?.OfType<WorldItem>().Any() ?? false, Is.False);
        });
    }
}
