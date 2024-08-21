namespace Labirint.Core.Tests;

[TestFixture]
public class RunnerAbilityTest
{
    [Test]
    [TestCase(null)]
    [TestCase(10)]
    public void AbilityActiveCorrectWorkTest(int? count)
    {
        TestAbility testAbility = new(count);

        RunnerAbility ability = new(testAbility);

        Assert.Multiple(() =>
        {
            Assert.That(ability.Active, Is.True);
        });

        for (int i = 0; i < testAbility.MoveCount; i++)
        {
            ability.Hit();
        }

        Assert.That(ability.Active, Is.EqualTo(testAbility.IsUnlimitedMoveCount));

        ability.Hit();

        Assert.That(testAbility.HitCount, Is.EqualTo(testAbility.IsUnlimitedMoveCount ? testAbility.HitCount : testAbility.MoveCount));
    }
}
