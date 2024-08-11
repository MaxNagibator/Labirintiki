using LabirintBlazorApp.Common;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class MazeWalls : MazeComponent
{
    [Parameter]
    [EditorRequired]
    public required int[,] Maze { get; set; }

    private int MazeWidth => Maze.GetLength(0);
    private int MazeHeight => Maze.GetLength(1);

    protected override string CanvasId => "mazeWallsCanvas";

    protected override async Task DrawAsync()
    {
        int fullWallOffset = WallWidth;
        int halfWallOffset = WallWidth / 2;

        DrawSequence drawSequence = new();

        drawSequence.ClearRect(0, 0, CanvasWidth, CanvasHeight);
        drawSequence.StrokeStyle(Parameters.Labyrinth.Color);
        drawSequence.LineWidth(WallWidth);

        for (int y = Vision.Start.Y; y < Vision.Finish.Y; y += 2)
        {
            for (int x = Vision.Start.X; x < Vision.Finish.X; x += 2)
            {
                (int drawX, int drawY) = Vision.GetDraw((x, y)) * HalfBoxSize;

                int topLeftX = drawX - HalfBoxSize;
                int topLeftY = drawY - HalfBoxSize;

                int bottomRightX = drawX + HalfBoxSize;
                int bottomRightY = drawY + HalfBoxSize;

                drawSequence.BeginPath();

                if (Maze[y, x - 1] == 0) // Left wall
                {
                    drawSequence.DrawLine(topLeftX + halfWallOffset, topLeftY, topLeftX + halfWallOffset, bottomRightY + fullWallOffset);
                }

                if (Maze[y - 1, x] == 0) // Top wall
                {
                    drawSequence.DrawLine(topLeftX, topLeftY + halfWallOffset, bottomRightX + fullWallOffset, topLeftY + halfWallOffset);
                }

                if (y == MazeHeight - 2 || x == MazeWidth - 2)
                {
                    if (Maze[y, x + 1] == 0) // Right wall
                    {
                        drawSequence.DrawLine(bottomRightX + halfWallOffset, topLeftY, bottomRightX + halfWallOffset, bottomRightY + fullWallOffset);
                    }

                    if (Maze[y + 1, x] == 0) // Bottom wall
                    {
                        drawSequence.DrawLine(topLeftX, bottomRightY + halfWallOffset, bottomRightX + fullWallOffset, bottomRightY + halfWallOffset);
                    }
                }

                drawSequence.Stroke();
            }
        }

        await Context.DrawSequenceAsync(drawSequence);
    }
}
