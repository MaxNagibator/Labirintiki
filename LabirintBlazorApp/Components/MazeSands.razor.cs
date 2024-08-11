using LabirintBlazorApp.Common;

namespace LabirintBlazorApp.Components;

public partial class MazeSands : MazeComponent
{
    protected override string CanvasId => "mazeSandsCanvas";

    protected override async Task DrawAsync()
    {
        DrawSequence drawSequence = new();
        drawSequence.ClearRect(0, 0, CanvasWidth, CanvasHeight);

        int entityBoxSize = HalfBoxSize * 2 - WallWidth;
        int offset = HalfBoxSize - entityBoxSize;

        for (int x = Vision.Start.X; x < Vision.Finish.X; x++)
        {
            for (int y = Vision.Start.Y; y < Vision.Finish.Y; y++)
            {
                if (Maze.Tiles[x, y].HasSand == false)
                {
                    continue;
                }

                (int drawX, int drawY) = (Vision.GetDraw((x, y)) - 1) * HalfBoxSize + WallWidth;

                int left = drawX - offset / 2;
                int top = drawY - offset;

                drawSequence.DrawImage("/images/sand.png", left, top, HalfBoxSize, HalfBoxSize);
            }
        }

        await Context.DrawSequenceAsync(drawSequence);
    }
}
