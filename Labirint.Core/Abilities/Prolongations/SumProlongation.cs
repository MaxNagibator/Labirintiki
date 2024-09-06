namespace Labirint.Core.Abilities.Prolongations;

/// <summary>
///     Увеличиваем время путём добавления максимального времени способности.
/// </summary>
public class SumProlongation : AbilityProlongation
{
    /// <inheritdoc />
    public override void Prolong(ref int? lostCount, int moveCount)
    {
        lostCount += moveCount;
    }
}
