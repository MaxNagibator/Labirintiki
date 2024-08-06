using LabirintBlazorApp.Common;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class MazeWalls : CanvasComponent
{
    [Parameter]
    [EditorRequired]
    public required int[,] Maze { get; set; }

    [Parameter]
    [EditorRequired]
    public required int HalfBoxSize { get; set; }

    [Parameter]
    [EditorRequired]
    public required int WallWidth { get; set; }

    private int MazeWidth => Maze.GetLength(0);
    private int MazeHeight => Maze.GetLength(1);

    protected override int CanvasWidth => Maze.GetLength(0) * HalfBoxSize;
    protected override int CanvasHeight => Maze.GetLength(1) * HalfBoxSize;

    protected override string CanvasId => "mazeWallsCanvas";

    protected override async Task DrawAsync()
    {
        int fullWallOffset = WallWidth;
        int halfWallOffset = WallWidth / 2;

        await Context.ClearRectAsync(0, 0, CanvasWidth, CanvasHeight);
        await Context.SetStrokeStyleAsync(Parameters.Labyrinth.Color);
        await Context.SetLineWidthAsync(WallWidth);

        List<DrawCommand> drawCommands = [];

        for (int y = 1; y < MazeHeight - 1; y += 2)
        {
            for (int x = 1; x < MazeWidth - 1; x += 2)
            {
                int topLeftY = y * HalfBoxSize - HalfBoxSize;
                int topLeftX = x * HalfBoxSize - HalfBoxSize;

                int bottomRightY = y * HalfBoxSize + HalfBoxSize;
                int bottomRightX = x * HalfBoxSize + HalfBoxSize;

                drawCommands.Add(new DrawCommand(DrawCommandType.BeginPath));

                if (Maze[y, x - 1] == 0) // Left wall
                {
                    drawCommands.Add(new DrawCommand(DrawCommandType.MoveTo, topLeftX + halfWallOffset, topLeftY));
                    drawCommands.Add(new DrawCommand(DrawCommandType.LineTo, topLeftX + halfWallOffset, bottomRightY + fullWallOffset));
                }

                if (Maze[y - 1, x] == 0) // Top wall
                {
                    drawCommands.Add(new DrawCommand(DrawCommandType.MoveTo, topLeftX, topLeftY + halfWallOffset));
                    drawCommands.Add(new DrawCommand(DrawCommandType.LineTo, bottomRightX + fullWallOffset, topLeftY + halfWallOffset));
                }

                if (y == MazeHeight - 2 || x == MazeWidth - 2)
                {
                    if (Maze[y, x + 1] == 0) // Right wall
                    {
                        drawCommands.Add(new DrawCommand(DrawCommandType.MoveTo, bottomRightX + halfWallOffset, topLeftY));
                        drawCommands.Add(new DrawCommand(DrawCommandType.LineTo, bottomRightX + halfWallOffset, bottomRightY + fullWallOffset));
                    }

                    if (Maze[y + 1, x] == 0) // Bottom wall
                    {
                        drawCommands.Add(new DrawCommand(DrawCommandType.MoveTo, topLeftX, bottomRightY + halfWallOffset));
                        drawCommands.Add(new DrawCommand(DrawCommandType.LineTo, bottomRightX + fullWallOffset, bottomRightY + halfWallOffset));
                    }
                }

                drawCommands.Add(new DrawCommand(DrawCommandType.Stroke));
            }
        }

        Logger.LogInformation("Количество команд на отрисовку (walls): {Count}", drawCommands.Count);

        await Context.DrawCommandsAsync(drawCommands);
    }

    public async Task UpdateAsync(List<(int, int)> updatedCells)
    {
        int entityBoxSize = HalfBoxSize * 2 - WallWidth;

        foreach ((int x, int y) in updatedCells)
        {
            int left = (x - 1) * HalfBoxSize + WallWidth;
            int top = (y - 1) * HalfBoxSize + WallWidth;

            if (IsDebug)
            {
                await Context.FillRectAsync(left, top, entityBoxSize, entityBoxSize);
            }
            else
            {
                await Context.ClearRectAsync(left, top, entityBoxSize, entityBoxSize);
            }
        }
    }
}
