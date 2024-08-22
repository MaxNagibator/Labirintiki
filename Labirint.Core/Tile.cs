namespace Labirint.Core;

/// <summary>
///     Клетка лабиринта.
/// </summary>
public class Tile
{
    /// <summary>
    ///     Предмет в клетке.
    /// </summary>
    public WorldItem? WorldItem { get; set; }

    /// <summary>
    ///     Направления стенок клетки.
    /// </summary>
    public Direction Walls { get; set; }

    /// <summary>
    ///     Является ли клетка выходом.
    /// </summary>
    public bool IsExit { get; set; }

    /// <summary>
    /// Сделать эффекты у клетки.
    /// </summary>
    public bool TempWoolYarn { get; set; }

    /// <summary>
    ///     Содержит ли клетка стенку с указанного направления.
    /// </summary>
    /// <param name="direction">Направление проверки.</param>
    /// <returns>True, если стенка присутствует; иначе false.</returns>
    public bool ContainsWall(Direction direction)
    {
        return Walls.HasFlag(direction);
    }

    /// <summary>
    ///     Добавить стенку в указанном направлении.
    /// </summary>
    /// <param name="direction">Направление добавления стенки.</param>
    public void AddWall(Direction direction)
    {
        if (Walls.HasFlag(direction))
        {
            return;
        }

        Walls |= direction;
    }

    /// <summary>
    ///     Удалить стенку в указанном направлении.
    /// </summary>
    /// <param name="direction">Направление удаления стенки.</param>
    public void RemoveWall(Direction direction)
    {
        if (Walls.HasFlag(direction) == false)
        {
            return;
        }

        Walls &= ~direction;
    }

    /// <summary>
    ///     Попробовать подобрать предмет, находящийся в клетке.
    /// </summary>
    /// <param name="item">Подобранный предмет, если операция успешна; иначе null.</param>
    /// <returns>True, если предмет был успешно подобран; иначе false.</returns>
    public bool TryPickUp(out WorldItem? item)
    {
        item = null;

        if (WorldItem == null || WorldItem.TryPickUp() == false)
        {
            return false;
        }

        item = WorldItem;
        WorldItem = null;
        return true;
    }

    public override string ToString()
    {
        return $"{nameof(Walls)}: {Walls}, {nameof(WorldItem)}: {WorldItem}, {nameof(IsExit)}: {IsExit}";
    }
}
