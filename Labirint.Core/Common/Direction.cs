namespace Labirint.Core.Common;

[Flags]
public enum Direction : byte
{
    None = 0,
    Left = 1,
    Top = 1 << 1,
    Right = 1 << 2,
    Bottom = 1 << 3,
    All = Left | Top | Right | Bottom
}
