using System.Diagnostics.CodeAnalysis;

namespace LabirintBlazorApp.Common;

public class DrawSequence
{
    private readonly List<Command> _commands = [];

    public int Count => _commands.Count;

    public void StrokeStyle(string color)
    {
        _commands.Add(new Command(Command.StrokeStyle)
        {
            Color = color
        });
    }

    public void LineWidth(double width)
    {
        _commands.Add(new Command(Command.LineWidth)
        {
            Width = width
        });
    }

    public void BeginPath()
    {
        _commands.Add(new Command(Command.BeginPath));
    }

    public void MoveTo(double x, double y)
    {
        _commands.Add(new Command(Command.MoveTo)
        {
            X = x,
            Y = y
        });
    }

    public void LineTo(double x, double y)
    {
        _commands.Add(new Command(Command.LineTo)
        {
            X = x,
            Y = y
        });
    }

    public void Stroke()
    {
        _commands.Add(new Command(Command.Stroke));
    }

    public void DrawLine(double topLeftX, double topLeftY, double bottomRightX, double bottomRightY)
    {
        MoveTo(topLeftX, topLeftY);
        LineTo(bottomRightX, bottomRightY);
    }

    public void DrawImage(string source, double left, double top, double width, double height)
    {
        _commands.Add(new Command(Command.DrawImage)
        {
            X = left,
            Y = top,
            Source = source,
            Width = width,
            Height = height
        });
    }

    public void ClearRect(double x, double y, double width, double height)
    {
        _commands.Add(new Command(Command.ClearRect)
        {
            X = x,
            Y = y,
            Width = width,
            Height = height
        });
    }

    public List<Command> ToList()
    {
        return [.._commands];
    }

    [method: SetsRequiredMembers]
    public class Command(int type)
    {
        public const int BeginPath = 0;
        public const int MoveTo = 1;
        public const int LineTo = 2;
        public const int Stroke = 3;
        public const int DrawImage = 4;
        public const int StrokeStyle = 5;
        public const int LineWidth = 6;
        public const int ClearRect = 7;

        public required int Type { get; init; } = type;
        public double X { get; init; }
        public double Y { get; init; }
        public string Source { get; init; } = string.Empty;
        public string Color { get; init; } = string.Empty;
        public double Width { get; init; }
        public double Height { get; init; }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, "
                   + $"{nameof(X)}: {X}, "
                   + $"{nameof(Y)}: {Y}, "
                   + $"{nameof(Source)}: {Source}, "
                   + $"{nameof(Color)}: {Color}, "
                   + $"{nameof(Width)}: {Width}, "
                   + $"{nameof(Height)}: {Height}";
        }
    }
}
