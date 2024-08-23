namespace LabirintBlazorApp.Components;

public partial class MazeEntities : MazeComponent
{
    protected override string CanvasId => "mazeEntitiesCanvas";

    protected override void DrawInner(int x, int y, DrawSequence sequence)
    {
        WorldItem? item = Maze[x, y].WorldItem;

        if (item == null)
        {
            return;
        }

        Position draw = Vision.GetDraw((x, y)) * BoxSize + WallWidth;
        (int offset, int entitySize) = AlignmentHelper.CalculateOffset(BoxSize, WallWidth, item.Scale);
        (int left, int top) = AlignmentHelper.CalculatePosition(item.Alignment, draw, offset);

        sequence.DrawImage(item.ImageSource, left, top, entitySize, entitySize);
    }
}
