using Labirint.Core.Abilities.Prolongations;

namespace Labirint.Core.Tests.Runners;

[TestFixture]
public class AbilityProlongationTests
{
    /// <summary>
    /// Тестирует, что SumProlongation.Prolong прибавляет продлеваемое количество ходов к оставшемуся счётчику.
    /// Проверяет, что после продления счётчик равен сумме исходного значения и переданного количества ходов.
    /// </summary>
    [Test]
    public void SumProlongationAddsMoveCountToRemainingChargesTest()
    {
        int? lostCount = 3;
        SumProlongation prolongation = new();

        prolongation.Prolong(ref lostCount, 5);

        Assert.That(lostCount, Is.EqualTo(8));
    }

    /// <summary>
    /// Тестирует, что ResetProlongation.Prolong сбрасывает оставшийся счётчик ходов до переданного максимума, а не суммирует его.
    /// Проверяет, что после продления счётчик равен переданному количеству ходов независимо от исходного значения.
    /// </summary>
    [Test]
    public void ResetProlongationSetsRemainingChargesToMaximumTest()
    {
        int? lostCount = 3;
        ResetProlongation prolongation = new();

        prolongation.Prolong(ref lostCount, 5);

        Assert.That(lostCount, Is.EqualTo(5));
    }
}
