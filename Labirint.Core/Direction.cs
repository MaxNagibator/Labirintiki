namespace Labirint.Core;

[Flags]
public enum Direction : byte
{
    Left = 1 << 1,
    Top = 1 << 2,
    Right = 1 << 3,
    Bottom = 1 << 4
}
