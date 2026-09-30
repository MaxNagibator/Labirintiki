using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Labirint.Web.Common.Drawing;

public class DrawSequence
{
    private readonly List<Command> _commands = [];
    private readonly List<double> _sprites = [];

    private string? _spriteSource;

    public void StrokeStyle(string color)
    {
        Add(new(Command.StrokeStyle)
        {
            Color = color,
        });
    }

    public void FillStyle(string color)
    {
        Add(new(Command.FillStyle)
        {
            Color = color,
        });
    }

    public void LineWidth(double width)
    {
        Add(new(Command.LineWidth)
        {
            Width = width,
        });
    }

    public void BeginPath()
    {
        Add(new(Command.BeginPath));
    }

    public void MoveTo(double x, double y)
    {
        Add(new(Command.MoveTo)
        {
            X = x,
            Y = y,
        });
    }

    public void LineTo(double x, double y)
    {
        Add(new(Command.LineTo)
        {
            X = x,
            Y = y,
        });
    }

    public void Stroke()
    {
        Add(new(Command.Stroke));
    }

    public void DrawLine(double topLeftX, double topLeftY, double bottomRightX, double bottomRightY)
    {
        MoveTo(topLeftX, topLeftY);
        LineTo(bottomRightX, bottomRightY);
    }

    public void FillRect(double x, double y, double width, double height)
    {
        Add(new(Command.FillRect)
        {
            X = x,
            Y = y,
            Width = width,
            Height = height,
        });
    }

    public void DrawImage(string source, double left, double top, double width, double height)
    {
        Add(new(Command.DrawImage)
        {
            X = left,
            Y = top,
            Source = source,
            Width = width,
            Height = height,
        });
    }

    public void ClearRect(double x, double y, double width, double height)
    {
        Add(new(Command.ClearRect)
        {
            X = x,
            Y = y,
            Width = width,
            Height = height,
        });
    }

    public void DrawSprite(string source, double sX, double sY, double sWidth, double sHeight, double dX, double dY, double dWidth, double dHeight)
    {
        if (_spriteSource != source)
        {
            FlushSprites();
            _spriteSource = source;
        }

        CollectionExtensions.AddRange(_sprites, [sX, sY, sWidth, sHeight, dX, dY, dWidth, dHeight]);
    }

    public void DrawSprite(string source, int row, int column, double sWidth, double sHeight, double dX, double dY, double dWidth, double dHeight)
    {
        DrawSprite(source, column * sWidth, row * sHeight, sWidth, sHeight,
            dX, dY, dWidth, dHeight);
    }

    public void DrawSprite(string source, int row, int column, double dX, double dY, double size)
    {
        DrawSprite(source, row, column, size, size,
            dX, dY, size, size);
    }

    public IReadOnlyList<Command> Build()
    {
        FlushSprites();
        return _commands;
    }

    private void Add(Command command)
    {
        FlushSprites();
        _commands.Add(command);
    }

    private void FlushSprites()
    {
        if (_spriteSource == null)
        {
            return;
        }

        _commands.Add(new(Command.DrawSprites)
        {
            Source = _spriteSource,
            Sprites = [.._sprites],
        });

        _sprites.Clear();
        _spriteSource = null;
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
        public const int FillRect = 8;
        public const int DrawSprites = 9;
        public const int FillStyle = 10;

        public required int Type { get; init; } = type;
        public double X { get; init; }
        public double Y { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Source { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Color { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double[]? Sprites { get; init; }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, "
                   + $"{nameof(X)}: {X}, "
                   + $"{nameof(Y)}: {Y}, "
                   + $"{nameof(Source)}: {Source}, "
                   + $"{nameof(Color)}: {Color}, "
                   + $"{nameof(Width)}: {Width}, "
                   + $"{nameof(Height)}: {Height}, "
                   + $"{nameof(Sprites)}: {Sprites?.Length / 8}";
        }
    }
}
