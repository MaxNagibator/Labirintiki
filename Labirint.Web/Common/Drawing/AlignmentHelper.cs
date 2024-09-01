using Labirint.Core.TileFeatures.Common;

namespace Labirint.Web.Common.Drawing;

public static class AlignmentHelper
{
    public static (int entitySize, Position drawPosition) GetAlignmentParameters(int boxSize, int wallWidth, double scale, Position position, Alignment alignment)
    {
        (int offset, int entitySize) = CalculateOffset(boxSize, alignment == Alignment.Stretch ? 0 : wallWidth, scale);
        Position drawPosition = CalculatePosition(alignment, position * boxSize + wallWidth, offset);

        return (entitySize, drawPosition);
    }

    public static (int entitySize, Position drawPosition) GetAlignmentParameters(int boxSize, int wallWidth, Position position, DrawingSettings settings)
    {
        return GetAlignmentParameters(boxSize, wallWidth, settings.Scale, position, settings.Alignment);
    }

    private static (int offset, int entitySize) CalculateOffset(int boxSize, int wallWidth, double scale)
    {
        int entityBoxSize = boxSize - wallWidth;
        int entitySize = (int)(entityBoxSize * scale);
        return (entitySize - entityBoxSize, entitySize);
    }

    private static Position CalculatePosition(Alignment alignment, Position position, int offset)
    {
        (int left, int top) = position;

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
