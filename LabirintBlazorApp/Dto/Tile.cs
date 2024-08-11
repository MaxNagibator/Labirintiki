
namespace LabirintBlazorApp.Dto;

/// <summary>
/// Клетка лабиринта.
/// </summary>
public class Tile()
{
    /// <summary>
    /// Содержит песочек.
    /// </summary>
    public bool HasSand { get; set; }

    public Direction Walls { get; set; }

    public bool IsExit { get; set; }

    /// <summary>
    /// Содержит стенку с указнного направления.
    /// </summary>
    /// <param name="direction">Направление проверки.</param>
    /// <returns></returns>
    public bool ContainsWall(Direction direction)
    {
        return Walls.HasFlag(direction);
    }
}

[Flags]
public enum Direction
{
    Left = 1 << 1,
    Top = 1 << 2,
    Right = 1 << 3,
    Bottom = 1 << 4,
}

