namespace LabirintBlazorApp.Dto;

public record struct Position(int X, int Y)
{
    public static Position operator +(Position a, Position b)
    {
        return (a.X + b.X, a.Y + b.Y);
    }

    public static Position operator +(Position a, int b)
    {
        return (a.X + b, a.Y + b);
    }

    public static Position operator -(Position a, Position b)
    {
        return (a.X - b.X, a.Y - b.Y);
    }

    public static Position operator -(Position a, int b)
    {
        return (a.X - b, a.Y - b);
    }

    public static Position operator *(Position a, int b)
    {
        return new Position(a.X * b, a.Y * b);
    }

    public static implicit operator (int X, int Y)(Position position)
    {
        return (position.X, position.Y);
    }

    public static implicit operator Position((int X, int Y) position)
    {
        return new Position(position.X, position.Y);
    }
}
