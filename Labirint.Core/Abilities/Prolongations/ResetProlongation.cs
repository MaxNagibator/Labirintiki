namespace Labirint.Core.Abilities.Prolongations;

/// <summary>
///     Устанавливаем время длительности способности до максимума.
/// </summary>
public class ResetProlongation : AbilityProlongation
{
    /// <inheritdoc />
    public override void Prolong(ref int? lostCount, int moveCount)
    {
        lostCount = moveCount;
    }
}
