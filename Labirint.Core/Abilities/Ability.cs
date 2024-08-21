
namespace Labirint.Core.Abilities;

/// <summary>
///     Способность.
/// </summary>
public abstract class Ability
{
    /// <summary>
    /// Наименование.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Изображение.
    /// </summary>
    public virtual string Icon => $"/images/abilities/{Name}.png";

    /// <summary>
    /// Безлимитная способность.
    /// </summary>
    public abstract bool IsUnlimitedMoveCount { get; }

    /// <summary>
    /// Количество зарядов. (один ход = -1 заряд).
    /// </summary>
    public abstract int MoveCount { get; }

    /// <summary>
    /// Что же делает способность.
    /// </summary>
    public abstract void Hit();
}
