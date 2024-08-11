using LabirintBlazorApp.Common;
using LabirintBlazorApp.Dto;

namespace LabirintBlazorApp.Components;

public partial class MazeWalls : MazeComponent
{
    protected override string CanvasId => "mazeWallsCanvas";

    protected override async Task DrawAsync()
    {
        int fullWallOffset = WallWidth / 2;
        int halfWallOffset = WallWidth / 4;

        DrawSequence drawSequence = new();

        drawSequence.ClearRect(0, 0, CanvasWidth, CanvasHeight);
        drawSequence.StrokeStyle(Parameters.Labyrinth.Color);
        drawSequence.LineWidth(WallWidth);

        for (int x = Vision.Start.X; x <= Vision.Finish.X; x++)
        {
            for (int y = Vision.Start.Y; y <= Vision.Finish.Y; y++)
            {
                (int drawX, int drawY) = Vision.GetDraw((x, y)) * HalfBoxSize;

                int topLeftX = drawX - HalfBoxSize;
                int topLeftY = drawY - HalfBoxSize;

                int bottomRightX = drawX + HalfBoxSize;
                int bottomRightY = drawY + HalfBoxSize;

                drawSequence.BeginPath();


                if (Maze.Tiles[x, y].ContainsWall(Direction.Left))
                {
                    drawSequence.DrawLine(topLeftX + halfWallOffset, topLeftY, topLeftX + halfWallOffset, bottomRightY + fullWallOffset);
                }

                if (Maze.Tiles[x, y].ContainsWall(Direction.Top))
                {
                    drawSequence.DrawLine(topLeftX, topLeftY + halfWallOffset, bottomRightX + fullWallOffset, topLeftY + halfWallOffset);
                }

                // нет смысла рисовать одну и ту же стенку дважды.
                if (x == Maze.Width - 1)
                {
                    if (Maze.Tiles[x, y].ContainsWall(Direction.Right))
                    {
                        drawSequence.DrawLine(bottomRightX + halfWallOffset, topLeftY, bottomRightX + halfWallOffset, bottomRightY + fullWallOffset);
                    }
                }

                if (y == Maze.Height - 1)
                {
                    if (Maze.Tiles[x, y].ContainsWall(Direction.Bottom))
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
