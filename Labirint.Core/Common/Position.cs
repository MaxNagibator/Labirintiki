using Labirint.Core.Extensions;

namespace Labirint.Core.Common;

public record struct Position(int X, int Y)
{
    public static Position operator +(Position left, Position right)
    {
        return (left.X + right.X, left.Y + right.Y);
    }

    public static Position operator +(Position left, Direction right)
    {
        return left + right.ToPosition();
    }

    public static Position operator +(Position left, int right)
    {
        return (left.X + right, left.Y + right);
    }

    public static Position operator -(Position left, Position right)
    {
        return (left.X - right.X, left.Y - right.Y);
    }

    public static Position operator -(Position left, Direction right)
    {
        return left - right.ToPosition();
    }

    public static Position operator -(Position left, int right)
    {
        return (left.X - right, left.Y - right);
    }

    public static Position operator *(Position left, int right)
    {
        return new Position(left.X * right, left.Y * right);
    }

    public static bool operator >(Position left, Position right)
    {
        return left.X > right.X && left.Y > right.Y;
    }

    public static bool operator >=(Position left, Position right)
    {
        return left.X >= right.X && left.Y >= right.Y;
    }

    public static bool operator <(Position left, Position right)
    {
        return left.X < right.X && left.Y < right.Y;
    }

    public static bool operator <=(Position left, Position right)
    {
        return left.X <= right.X && left.Y <= right.Y;
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
