using Labirint.Core.Items;

namespace Labirint.Core.Tests.Items;

[TestFixture]
public class ItemUseTests : LabyrinthTestsBase
{
    private static readonly Position Origin = (1, 1);

    /// <summary>
    /// Тестирует, что предмет, которому нужно направление, переживает применение без направления вместо падения.
    /// Проверяет, что применение не бросает исключение и стена справа от бегуна остаётся целой.
    /// </summary>
    /// <param name="direction">Направление применения или null.</param>
    [TestCase(null)]
    [TestCase(Direction.None)]
    public void UseWithoutDirectionDoesNothingTest(Direction? direction)
    {
        var hammer = Labyrinth.Runner.Inventory.AllItems.OfType<Hammer>().Single();

        Labyrinth.CreateWall(Origin, Direction.Right);

        Assert.DoesNotThrow(() => hammer.Use(Origin, direction, Labyrinth));

        Assert.That(Labyrinth[Origin].ContainsWall(Direction.Right), Is.True);
    }
}
