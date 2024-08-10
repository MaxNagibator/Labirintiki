using LabirintBlazorApp.Common;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class MazeSands : MazeComponent
{
    [Parameter]
    [EditorRequired]
    public required int[,] Sand { get; set; }

    protected override string CanvasId => "mazeSandsCanvas";

    protected override async Task DrawAsync()
    {
        DrawSequence drawSequence = new();
        drawSequence.ClearRect(0, 0, CanvasWidth, CanvasHeight);

        int entityBoxSize = HalfBoxSize * 2 - WallWidth;
        int offset = HalfBoxSize - entityBoxSize;

        for (int y = Vision.Start.Y; y < Vision.Finish.Y; y += 2)
        {
            for (int x = Vision.Start.X; x < Vision.Finish.X; x += 2)
            {
                if (Sand[y, x] != 0)
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
