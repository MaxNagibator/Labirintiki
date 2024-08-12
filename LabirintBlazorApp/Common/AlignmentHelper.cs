using LabirintBlazorApp.Dto;

namespace LabirintBlazorApp.Common;

public static class AlignmentHelper
{
    public static int CalculateOffset(int boxSize, int wallWidth)
    {
        int entityBoxSize = boxSize - wallWidth;
        int entitySize = entityBoxSize / 2;
        return entitySize - entityBoxSize;
    }

    public static Position CalculatePosition(Alignment alignment, Position draw, int boxSize, int wallWidth)
    {
        return CalculatePosition(alignment, draw, CalculateOffset(boxSize, wallWidth));
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
