using Labirint.Core.TileFeatures.Common;

namespace Labirint.Web.Common.Extensions;

public static class DrawSequenceExtensions
{
    public static void DrawImage(this DrawSequence sequence, DrawingSettings settings, int boxSize, int wallWidth, Position position)
    {
        (int entitySize, Position drawPosition) = AlignmentHelper.GetAlignmentParameters(boxSize, wallWidth, position, settings);
        sequence.DrawImage(settings.ImageSource, drawPosition.X, drawPosition.Y, entitySize, entitySize);
    }
}
