namespace LabirintBlazorApp.Dto;

/// <summary>
///     Расширения для перечисления Direction.
/// </summary>
public static class DirectionExtensions
{
    /// <summary>
    ///     Получить соседнюю позицию в заданном направлении.
    /// </summary>
    /// <param name="direction">Направление.</param>
    /// <param name="position">Текущая позиция.</param>
    /// <returns>Соседняя позиция.</returns>
    public static Position GetAdjacentPosition(this Direction direction, Position position)
    {
        return direction.ToPosition() + position;
    }

    /// <summary>
    ///     Получить противоположное направление.
    /// </summary>
    /// <param name="direction">Направление.</param>
    /// <returns>Противоположное направление.</returns>
    public static Direction GetOppositeDirection(this Direction direction)
    {
        return direction switch
        {
            Direction.Left => Direction.Right,
            Direction.Top => Direction.Bottom,
            Direction.Right => Direction.Left,
            Direction.Bottom => Direction.Top,
            var _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    /// <summary>
    ///     Преобразовать направление в необходимое смещение для перемещения.
    /// </summary>
    /// <param name="direction">Направление.</param>
    /// <returns>Смещение в виде позиции.</returns>
    public static Position ToPosition(this Direction direction)
    {
        return direction switch
        {
            Direction.Left => (-1, 0),
            Direction.Top => (0, -1),
            Direction.Right => (1, 0),
            Direction.Bottom => (0, 1),
            var _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}
