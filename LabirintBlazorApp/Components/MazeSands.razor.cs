using LabirintBlazorApp.Common;

namespace LabirintBlazorApp.Components;

public partial class MazeSands : MazeComponent
{
    private int _entitySize;
    private int _offset;

    protected override string CanvasId => "mazeSandsCanvas";

    protected override void OnParametersSetInner()
    {
        int entityBoxSize = BoxSize - WallWidth;
        _entitySize = entityBoxSize / 2;
        _offset = _entitySize - entityBoxSize;
    }

    protected override async Task DrawAsync()
    {
        DrawSequence drawSequence = new();
        drawSequence.ClearRect(0, 0, CanvasWidth, CanvasHeight);

        for (int x = Vision.Start.X; x <= Vision.Finish.X; x++)
        {
            for (int y = Vision.Start.Y; y <= Vision.Finish.Y; y++)
            {
                if (Maze[x, y].HasSand == false)
                {
                    continue;
                }

                (int drawX, int drawY) = Vision.GetDraw((x, y)) * BoxSize + WallWidth;

                int left = drawX - _offset / 2;
                int top = drawY - _offset;

                drawSequence.DrawImage("/images/sand.png", left, top, _entitySize, _entitySize);
            }
        }

        await Context.DrawSequenceAsync(drawSequence);
    }
}
