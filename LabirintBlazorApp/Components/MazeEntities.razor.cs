using Labirint.Core.TileFeatures.Base;
using Labirint.Core.TileFeatures.Common;

namespace LabirintBlazorApp.Components;

public partial class MazeEntities : MazeComponent
{
    protected override string CanvasId => "mazeEntitiesCanvas";

    protected override void DrawInner(int x, int y, DrawSequence sequence)
    {
        IEnumerable<TileFeature>? tileFeatures = Maze[x, y]
            .Features
            ?.Where(feature => feature.DrawingSettings != null)
            .DistinctBy(feature => feature.DrawingSettings)
            .OrderBy(feature => feature.DrawingSettings!.Order);

        foreach (TileFeature feature in tileFeatures ?? [])
        {
            DrawingSettings settings = feature.DrawingSettings!;

            Position draw = Vision.GetDraw((x, y)) * BoxSize + WallWidth;
            (int offset, int entitySize) = AlignmentHelper.CalculateOffset(BoxSize, settings.Alignment == Alignment.Stretch ? 0 : WallWidth, settings.Scale);
            (int left, int top) = AlignmentHelper.CalculatePosition(settings.Alignment, draw, offset);

            sequence.DrawImage(settings.ImageSource, left, top, entitySize, entitySize);
        }
    }
}
