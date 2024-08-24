namespace LabirintBlazorApp.Components;

public partial class MazeFloor : MazeComponent
{
    protected override string CanvasId => "mazeFloorCanvas";

    protected override string StrokeStyle => "black";

    protected override void DrawInner(int x, int y, DrawSequence sequence)
    {
        Position topLeft = Vision.GetDraw((x, y)) * BoxSize;
        int id = DetermineTileId(x, y, Vision.Start, Vision.Finish);

        // Альтернативный вариант для отображение реальных границ лабиринта, а не границ области видимости
        // int id = DetermineTileId(x, y, (0, 0), (Position)(Maze.Width, Maze.Height) - 1);

        sequence.DrawRect(topLeft.X, topLeft.Y, BoxSize + WallWidth, BoxSize + WallWidth);
        sequence.DrawImage($"/images/tiles/tile00{id}.png", topLeft.X, topLeft.Y, BoxSize, BoxSize);
    }

    private int DetermineTileId(int x, int y, Position start, Position finish)
    {
        if (x == start.X && y == start.Y)
        {
            return 0;
        }

        if (x == finish.X && y == finish.Y)
        {
            return 8;
        }

        if (y == start.Y)
        {
            return x == finish.X ? 2 : 1;
        }

        if (x == start.X)
        {
            return y == finish.Y ? 6 : 3;
        }

        if (x == finish.X)
        {
            return 5;
        }

        if (y == finish.Y)
        {
            return 7;
        }

        return 4;
    }
}
