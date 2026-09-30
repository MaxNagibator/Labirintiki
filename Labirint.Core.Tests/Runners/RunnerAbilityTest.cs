using Labirint.Core.Abilities.Base;
using Labirint.Core.Abilities.Prolongations;
using Labirint.Core.Abilities.Prolongations.Base;

namespace Labirint.Core.Tests.Runners;

internal class TestAbility(int? moveCount = null) : Ability
{
    public override string Name => "TestAbility";
    public override string DisplayName => "TestAbility";

    public override int? MoveCount { get; } = moveCount;
    public int HitCount { get; private set; }

    public override AbilityProlongation Prolongation => new SumProlongation();

    public override void Hit(Tile tile, Direction direction)
    {
        HitCount++;
    }
}

[TestFixture]
public class RunnerAbilityTest : LabyrinthTestsBase
{
    /// <summary>
    /// Тестирует, что RunnerAbility деактивируется после исчерпания MoveCount ударов и не деактивируется при безлимитном MoveCount.
    /// Проверяет, что после MoveCount ударов активность способности совпадает с IsUnlimitedMoveCount, а число фактических вызовов Hit ограничено MoveCount у способностей с лимитом.
    /// </summary>
    /// <param name="count">Лимит ходов способности или null для безлимитной</param>
    [Test]
    [TestCase(null)]
    [TestCase(10)]
    public void AbilityActiveCorrectWorkTest(int? count)
    {
        Tile tile = new(Labyrinth);
        TestAbility testAbility = new(count);
        RunnerAbility ability = new(testAbility);

        Assert.That(ability.Active, Is.True);

        for (var i = 0; i < testAbility.MoveCount; i++)
        {
            ability.Hit(tile, Direction.None);
        }

        Assert.That(ability.Active, Is.EqualTo(testAbility.IsUnlimitedMoveCount));

        ability.Hit(tile, Direction.None);

        Assert.That(testAbility.HitCount, Is.EqualTo(testAbility.MoveCount ?? 1));
    }
}
