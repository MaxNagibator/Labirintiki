namespace Labirint.Core.Items.Base;

/// <summary>
/// Возможности подобравшего, доступные предмету в момент подбора.
/// </summary>
public interface IPickupContext
{
    /// <summary>
    /// Положить предмет в инвентарь.
    /// </summary>
    /// <param name="item">Предмет.</param>
    /// <param name="count">Количество штук.</param>
    /// <returns>True, если предмет поместился в инвентарь; иначе false.</returns>
    bool TryStore(Item item, int count);

    /// <summary>
    /// Начислить очки.
    /// </summary>
    /// <param name="amount">Количество очков.</param>
    void AddScore(int amount);

    /// <summary>
    /// Применить предмет немедленно, минуя инвентарь.
    /// </summary>
    /// <param name="item">Предмет.</param>
    void Use(Item item);
}
