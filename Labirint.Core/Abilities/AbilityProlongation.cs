namespace Labirint.Core.Abilities;

/// <summary>
///     Продление способности.
/// </summary>
/// <remarks>
///     Если способность уже активна, как будем её продлевать.
/// </remarks>
public enum AbilityProlongation
{
    /// <summary>
    /// Увеличиваем время путём добавления максимального времени способности.
    /// </summary>
    Sum = 0,

    /// <summary>
    /// Устанавливаем время длительности способности до максимума.
    /// </summary>
    Reset = 1,
}
