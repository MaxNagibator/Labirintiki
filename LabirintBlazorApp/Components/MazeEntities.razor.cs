using Labirint.Core.TileFeatures;

namespace LabirintBlazorApp.Components;

public partial class MazeEntities : MazeComponent
{
    protected override string CanvasId => "mazeEntitiesCanvas";

    protected override void DrawInner(int x, int y, DrawSequence sequence)
    {
        foreach (var feature in Maze[x, y].Features)
        {
            Position draw = Vision.GetDraw((x, y)) * BoxSize + WallWidth;
            (int offset, int entitySize) = AlignmentHelper.CalculateOffset(BoxSize, WallWidth, feature.Scale);
            (int left, int top) = AlignmentHelper.CalculatePosition(feature.Alignment, draw, offset);

            sequence.DrawImage(feature.ImageSource, left, top, entitySize, entitySize);
        }
    }
}
