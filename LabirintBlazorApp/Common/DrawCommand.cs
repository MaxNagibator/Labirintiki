using System.Diagnostics.CodeAnalysis;

namespace LabirintBlazorApp.Common;

[method: SetsRequiredMembers]
public class DrawCommand(DrawCommandType type, int x = 0, int y = 0)
{
    [SetsRequiredMembers]
    public DrawCommand(DrawCommandType type, string source, int x, int y, double size) : this(type, x, y)
    {
        Source = source;
        Size = size;
    }

    [SetsRequiredMembers]
    public DrawCommand(DrawCommandType type, string color) : this(type)
    {
        Color = color;
    }

    [SetsRequiredMembers]
    public DrawCommand(DrawCommandType type, double size) : this(type)
    {
        Size = size;
    }

    public required DrawCommandType Type { get; set; } = type;
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public string? Source { get; set; }
    public string? Color { get; set; }
    public double Size { get; set; }
}
