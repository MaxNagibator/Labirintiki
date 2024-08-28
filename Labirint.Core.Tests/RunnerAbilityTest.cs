using Labirint.Core.Abilities.Base;

namespace Labirint.Core.Tests;

internal class TestAbility(int? moveCount = null) : Ability
{
    public override string Name => "TestAbility";
    public override string DisplayName => "TestAbility";

    public override int? MoveCount { get; } = moveCount;
    public int HitCount { get; private set; }

    public override void Hit(Tile tile)
    {
        HitCount++;
    }
}

[TestFixture]
public class RunnerAbilityTest : BaseLabyrinthTests
{
    [Test]
    [TestCase(null)]
    [TestCase(10)]
    public void AbilityActiveCorrectWorkTest(int? count)
    {
        Tile tile = new(Labyrinth);
        TestAbility testAbility = new(count);
        RunnerAbility ability = new(testAbility);

        Assert.That(ability.Active, Is.True);

        for (int i = 0; i < testAbility.MoveCount; i++)
        {
            ability.Hit(tile);
        }

        Assert.That(ability.Active, Is.EqualTo(testAbility.IsUnlimitedMoveCount));

        ability.Hit(tile);

        Assert.That(testAbility.HitCount, Is.EqualTo(testAbility.IsUnlimitedMoveCount ? testAbility.HitCount : testAbility.MoveCount));
    }
}
