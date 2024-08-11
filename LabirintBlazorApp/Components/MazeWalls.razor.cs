using LabirintBlazorApp.Common;
using LabirintBlazorApp.Dto;

namespace LabirintBlazorApp.Components;

public partial class MazeWalls : MazeComponent
{
    private int _height;
    private int _width;

    protected override string CanvasId => "mazeWallsCanvas";

    protected override void OnParametersSetInner()
    {
        _width = WallWidth;
        _height = BoxSize + WallWidth;
    }

    protected override async Task DrawAsync()
    {
        // При рисовании на canvas, ширина линии симметрична центру,
        // то есть при рисовании из (0,0) в (0,10) получится линии в половину ширины,
        // в итоге линия - это будет прямоугольник (0,0) в (ширина/2, 10),
        // поэтому нужны были смешение в половину ширины линии.
        // Теперь для упрощения понимания рисуются не линии, а просто прямоугольники.

        DrawSequence drawSequence = new();

        drawSequence.ClearRect(0, 0, CanvasWidth, CanvasHeight);
        drawSequence.StrokeStyle(Parameters.Labyrinth.Color);

        for (int x = Vision.Start.X; x <= Vision.Finish.X; x++)
        {
            for (int y = Vision.Start.Y; y <= Vision.Finish.Y; y++)
            {
                Position topLeft = Vision.GetDraw((x, y)) * BoxSize;
                Position bottomRight = topLeft + BoxSize;

                if (Maze[x, y].ContainsWall(Direction.Left))
                {
                    drawSequence.DrawRect(topLeft.X, topLeft.Y, _width, _height);
                }

                if (Maze[x, y].ContainsWall(Direction.Top))
                {
                    drawSequence.DrawRect(topLeft.X, topLeft.Y, _height, _width);
                }

                // нет смысла рисовать одну и ту же стенку дважды.
                if (x == Maze.Width - 1)
                {
                    if (Maze[x, y].ContainsWall(Direction.Right))
                    {
                        drawSequence.DrawRect(bottomRight.X, topLeft.Y, _width, _height);
                    }
                }

                if (y == Maze.Height - 1)
                {
                    if (Maze[x, y].ContainsWall(Direction.Bottom))
                    {
                        drawSequence.DrawRect(topLeft.X, bottomRight.Y, _height, _width);
                    }
                }
            }
        }

        await Context.DrawSequenceAsync(drawSequence);
    }
}
