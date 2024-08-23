namespace Labirint.Core;

/// <summary>
///     Клетка лабиринта.
/// </summary>
public class Tile
{
    /// <summary>
    ///     Особенности клетки.
    /// </summary>
    public List<TileFeature>? Features { get; set; }

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

    public void AddFeature(TileFeature feature)
    {
        Features ??= [];

        Features.Add(feature);
    }

    /// <summary>
    ///     Попробовать подобрать предмет, находящийся в клетке.
    /// </summary>
    /// <param name="item">Подобранный предмет, если операция успешна; иначе null.</param>
    /// <returns>True, если предмет был успешно подобран; иначе false.</returns>
    public bool TryPickUp(out TileFeature? item)
    {
        item = null;

        if (Features == null)
        {
            return false;
        }

        List<TileFeature> newFeatures = [];

        foreach (TileFeature feature in Features)
        {
            if (feature.TryPickUp())
            {
                item = feature;

                if (feature.RemoveAfterSuccessPickUp)
                {
                }
                else
                {
                    newFeatures.Add(feature);
                }
            }
            else
            {
                newFeatures.Add(feature);
            }
        }

        Features = newFeatures;

        return item != null;
    }

    /// <summary>
    ///     Приведет ли добавление стены по указанному направлению к созданию ячейки со всеми закрытыми сторонами.
    /// </summary>
    /// <param name="direction">Направление стены</param>
    /// <returns>True, если не приведет, иначе False</returns>
    public bool CanAddWall(Direction direction)
    {
        return direction != Direction.None && (ContainsWall(direction) || (Walls | direction) == Direction.All) == false;
    }

    public override string ToString()
    {
        return $"{nameof(Walls)}: {Walls}, {nameof(Features)}: {Features?.Count}, {nameof(IsExit)}: {IsExit}";
    }
}
