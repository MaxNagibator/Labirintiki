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
    ///     Сделать эффекты у клетки.
    /// </summary>
    public bool TempWoolYarn { get; set; }

    /// <summary>
    ///     Содержит ли клетка стенку с указанного направления.
    /// </summary>
    /// <param name="direction">Направление проверки.</param>
    /// <returns>True, если стенка присутствует; иначе false.</returns>
    public bool ContainsWall(Direction direction)
    {
        return (Walls & direction) != 0;
    }

    /// <summary>
    ///     Добавить стенку в указанном направлении.
    /// </summary>
    /// <param name="direction">Направление добавления стенки.</param>
    public void AddWall(Direction direction)
    {
        if (ContainsWall(direction))
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
        if (ContainsWall(direction) == false)
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

    /// <summary>
    ///     Приведет ли добавление стены по указанному направлению к созданию ячейки со всеми закрытыми сторонами.
    /// </summary>
    /// <param name="direction">Направление стены</param>
    /// <returns>True, если не приведет, иначе False</returns>
    public bool CanAddWall(Direction direction)
    {
        return (ContainsWall(direction) || (Walls | direction) == Direction.All) == false;
    }

    public override string ToString()
    {
        return $"{nameof(Walls)}: {Walls}, {nameof(WorldItem)}: {WorldItem}, {nameof(IsExit)}: {IsExit}";
    }
}
