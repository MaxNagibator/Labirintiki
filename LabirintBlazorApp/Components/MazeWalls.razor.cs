﻿using Labirint.Core.Common;

namespace LabirintBlazorApp.Components;

public partial class MazeWalls : MazeComponent
{
    private int _width;
    private int _height;

    protected override string CanvasId => "mazeWallsCanvas";

    protected override void OnParametersSetInner()
    {
        _width = WallWidth;
        _height = BoxSize + WallWidth;
    }

    protected override void DrawInner(int x, int y, DrawSequence sequence)
    {
        // При рисовании на canvas, ширина линии симметрична центру,
        // то есть при рисовании из (0,0) в (0,10) получится линии в половину ширины,
        // в итоге линия - это будет прямоугольник (0,0) в (ширина/2, 10),
        // поэтому нужны были смешение в половину ширины линии.
        // Теперь для упрощения понимания рисуются не линии, а просто прямоугольники.

        Position topLeft = Vision.GetDraw((x, y)) * BoxSize;
        Position bottomRight = topLeft + BoxSize;

        if (Maze[x, y].ContainsWall(Direction.Left))
        {
            sequence.DrawRect(topLeft.X, topLeft.Y, _width, _height);
        }

        if (Maze[x, y].ContainsWall(Direction.Top))
        {
            sequence.DrawRect(topLeft.X, topLeft.Y, _height, _width);
        }

        // нет смысла рисовать одну и ту же стенку дважды.
        // Без них улучшается внешний вид и нет необходимости в костыле с областью видимости в MazeComponent
        // if (x == Maze.Width - 1)
        {
            if (Maze[x, y].ContainsWall(Direction.Right))
            {
                sequence.DrawRect(bottomRight.X, topLeft.Y, _width, _height);
            }
        }

        // if (y == Maze.Height - 1)
        {
            if (Maze[x, y].ContainsWall(Direction.Bottom))
            {
                sequence.DrawRect(topLeft.X, bottomRight.Y, _height, _width);
            }
        }

        if (Maze[x, y].TempWoolYarn)
        {
            sequence.DrawRect(topLeft.X+10, topLeft.Y+10, _height/4, _width*4);
        }
    }
}
