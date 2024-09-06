namespace Labirint.Core.Abilities.Prolongations.Base;

/// <summary>
///     Базовый класс для продления способности.
/// </summary>
/// <remarks>Если способность уже активна, как будем её продлевать.</remarks>
public abstract class AbilityProlongation
{
    /// <summary>
    ///     Продлить способность.
    /// </summary>
    /// <param name="lostCount">Текущее количество оставшихся ходов.</param>
    /// <param name="moveCount">Максимальное количество ходов способности.</param>
    public abstract void Prolong(ref int? lostCount, int moveCount);
}
