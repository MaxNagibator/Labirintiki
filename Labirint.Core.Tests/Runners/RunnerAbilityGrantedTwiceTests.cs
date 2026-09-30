using Labirint.Core.Abilities;

namespace Labirint.Core.Tests.Runners;

[TestFixture]
public class RunnerAbilityGrantedTwiceTests : LabyrinthTestsBase
{
    /// <summary>
    /// Тестирует, что Runner.AddAbility при повторной выдаче WoolYarnAbility суммирует оставшиеся заряды через SumProlongation вместо повторной выдачи способности.
    /// Проверяет, что после повторной выдачи LostCount увеличивается на исходный максимум ходов, а у бегуна остаётся одна способность.
    /// </summary>
    [Test]
    public void SumProlongationAccumulatesRemainingChargesWhenGrantedAgainTest()
    {
        Labyrinth.Runner.AddAbility(new WoolYarnAbility());
        var ability = Labyrinth.Runner.Abilities.Single();
        var tile = Labyrinth[0, 0];

        for (var i = 0; i < 10; i++)
        {
            ability.Hit(tile, Direction.Left);
        }

        Assert.That(ability.LostCount, Is.EqualTo(90));

        Labyrinth.Runner.AddAbility(new WoolYarnAbility());

        Assert.Multiple(() =>
        {
            Assert.That(ability.LostCount, Is.EqualTo(190));
            Assert.That(Labyrinth.Runner.Abilities, Has.Count.EqualTo(1));
        });
    }

    /// <summary>
    /// Тестирует, что Runner.AddAbility при повторной выдаче WalkThroughWallsAbility после исчерпания зарядов заново активирует способность через ResetProlongation вместо повторной выдачи способности.
    /// Проверяет, что после повторной выдачи способность снова активна, LostCount сброшен до количества уже совершённых ударов, а у бегуна остаётся одна способность.
    /// </summary>
    [Test]
    public void ResetProlongationRestoresRemainingChargesToMaximumWhenGrantedAgainTest()
    {
        Labyrinth.Runner.AddAbility(new WalkThroughWallsAbility());
        var ability = Labyrinth.Runner.Abilities.Single();
        var tile = Labyrinth[0, 0];

        for (var i = 0; i < 5; i++)
        {
            ability.Hit(tile, Direction.None);
        }

        Assert.That(ability.Active, Is.False);

        Labyrinth.Runner.AddAbility(new WalkThroughWallsAbility());

        Assert.Multiple(() =>
        {
            Assert.That(ability.Active, Is.True);
            Assert.That(ability.LostCount, Is.EqualTo(5));
            Assert.That(Labyrinth.Runner.Abilities, Has.Count.EqualTo(1));
        });
    }
}
