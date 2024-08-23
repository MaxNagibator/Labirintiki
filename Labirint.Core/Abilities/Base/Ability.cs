namespace Labirint.Core.Abilities.Base;

/// <summary>
///     Способность.
/// </summary>
public abstract class Ability
{
    /// <summary>
    ///     Наименование.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    ///     Описание.
    /// </summary>
    public abstract string DisplayName { get; }

    /// <summary>
    ///     Изображение.
    /// </summary>
    public virtual string Icon => $"/images/abilities/{Name}.png";

    /// <summary>
    ///     Безлимитная способность.
    /// </summary>
    public bool IsUnlimitedMoveCount => MoveCount == null;

    /// <summary>
    ///     Количество зарядов. (один ход = -1 заряд).
    /// </summary>
    public virtual int? MoveCount => null;

    /// <summary>
    ///     Что же делает способность.
    /// </summary>
    /// <param name="tile">Клетка в которую мы перешли.</param>
    public abstract void Hit(Tile tile);
}
