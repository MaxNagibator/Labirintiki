namespace LabirintBlazorApp.Dto;

public record struct Position(int X, int Y)
{
    public static implicit operator (int X, int Y)(Position position)
    {
        return (position.X, position.Y);
    }

    public static implicit operator Position((int X, int Y) position)
    {
        return new Position(position.X, position.Y);
    }
}
