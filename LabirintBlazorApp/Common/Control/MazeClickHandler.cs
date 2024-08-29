using System.Drawing;

namespace LabirintBlazorApp.Common.Control;

public class MazeClickHandler
{
    private readonly Rectangle _left;
    private readonly Rectangle _top;
    private readonly Rectangle _right;
    private readonly Rectangle _bottom;

    public MazeClickHandler(int canvasWidth, int canvasHeight)
    {
        int xStep = canvasWidth / 4;
        int yStep = canvasHeight / 4;

        _left = new Rectangle(0, yStep, xStep, yStep * 2);
        _top = new Rectangle(xStep, 0, xStep * 2, yStep);
        _right = new Rectangle(xStep * 3, yStep, xStep, yStep * 2);
        _bottom = new Rectangle(xStep, yStep * 3, xStep * 2, yStep);
    }

    public Direction GetDirection(int x, int y)
    {
        if (_left.Contains(x, y))
        {
            return Direction.Left;
        }

        if (_top.Contains(x, y))
        {
            return Direction.Top;
        }

        if (_right.Contains(x, y))
        {
            return Direction.Right;
        }

        if (_bottom.Contains(x, y))
        {
            return Direction.Bottom;
        }

        return Direction.None;
    }
}
