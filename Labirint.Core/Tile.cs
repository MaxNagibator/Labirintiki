namespace Labirint.Core;

/// <summary>
///     Клетка лабиринта.
/// </summary>
public class Tile
{
    /// <summary>
    ///     Тип предмета в клетке.
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
    ///     Содержит ли клетка стенку с указанного направления.
    /// </summary>
    /// <param name="direction">Направление проверки.</param>
    /// <returns>True, если стенка присутствует; иначе false.</returns>
    public bool ContainsWall(Direction direction)
    {
        return Walls.HasFlag(direction);
    }

    /// <summary>
    ///     Добавляет стенку в указанном направлении.
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
    ///     Удаляет стенку в указанном направлении.
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

    public bool TryPickUp(out WorldItem? item)
    {
        item = null;

        if (WorldItem == null)
        {
            return false;
        }

        if (WorldItem.TryPickUp() == false)
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
