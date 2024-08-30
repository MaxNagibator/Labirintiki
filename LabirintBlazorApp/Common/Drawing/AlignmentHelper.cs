namespace LabirintBlazorApp.Common.Drawing;

public static class AlignmentHelper
{
    public static (int offset, int entitySize) CalculateOffset(int boxSize, int wallWidth, double scale)
    {
        int entityBoxSize = boxSize - wallWidth;
        int entitySize = (int)(entityBoxSize * scale);
        return (entitySize - entityBoxSize, entitySize);
    }

    public static Position CalculatePosition(Alignment alignment, Position draw, int offset)
    {
        (int left, int top) = draw;

        switch (alignment)
        {
            case Alignment.TopLeft:
                break;

            case Alignment.TopCenter:
                left -= offset / 2;
                break;

            case Alignment.TopRight:
                left -= offset;
                break;

            case Alignment.CenterLeft:
                top -= offset / 2;
                break;

            case Alignment.Stretch:
            case Alignment.Center:
                left -= offset / 2;
                top -= offset / 2;
                break;

            case Alignment.CenterRight:
                left -= offset;
                top -= offset / 2;
                break;

            case Alignment.BottomLeft:
                top -= offset;
                break;

            case Alignment.BottomCenter:
                left -= offset / 2;
                top -= offset;
                break;

            case Alignment.BottomRight:
                left -= offset;
                top -= offset;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
        }

        return (left, top);
    }
}
