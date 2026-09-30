namespace Labirint.Web.Components;

public partial class MazeFloor : MazeComponent
{
    protected override string CanvasId => "mazeFloorCanvas";

    protected override string FillStyle => "black";

    protected override void DrawInner(int x, int y, DrawSequence sequence)
    {
        var topLeft = Vision.GetDraw((x, y)) * BoxSize;

        sequence.FillRect(topLeft.X, topLeft.Y, BoxSize + WallWidth, BoxSize + WallWidth);

        var tileSize = BoxSize / 2;

        var quarters = GetTile(x, y);
        ReadOnlySpan<int> tile = [quarters.TopLeft, quarters.BottomLeft, quarters.TopRight, quarters.BottomRight];

        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < 2; j++)
            {
                var left = topLeft.X + i * tileSize;
                var top = topLeft.Y + j * tileSize;
                var sprite = tile[i * 2 + j];

                sequence.DrawSprite("images/tiles/floor.png", sprite / 6, sprite % 6, left, top, tileSize);
            }
        }
    }

    private (int TopLeft, int TopRight, int BottomLeft, int BottomRight) GetTile(int x, int y)
    {
        int? topLeft = null;
        int? bottomLeft = null;
        int? topRight = null;
        int? bottomRight = null;

        if (Maze[x, y].ContainsWall(Direction.Left))
        {
            topLeft = 12;
            bottomLeft = 18;
        }

        if (Maze[x, y].ContainsWall(Direction.Top))
        {
            if (topLeft == null)
            {
                topLeft = 2;
            }
            else
            {
                topLeft = 0;
                bottomLeft = 6;
            }

            topRight = topLeft + 1;
        }

        if (Maze[x, y].ContainsWall(Direction.Right))
        {
            if (topRight == null)
            {
                topRight = 17;
            }
            else
            {
                topRight = 5;

                if (topLeft != 0)
                {
                    topLeft = 4;
                }
            }

            bottomRight = topRight + 6;
        }

        if (Maze[x, y].ContainsWall(Direction.Bottom))
        {
            bottomLeft = bottomLeft == null ? 32 : 30;

            if (bottomRight == null)
            {
                bottomRight = bottomLeft + 1;
            }
            else
            {
                bottomRight = 35;

                if (topRight != 5)
                {
                    topRight = 29;
                }
            }
        }

        if (topLeft != null)
        {
            topRight ??= topLeft + 1;
            bottomLeft ??= topLeft + 6;
        }

        if (topRight != null)
        {
            bottomRight ??= topRight + 6;
            topLeft ??= topRight - 1;
        }

        if (bottomLeft != null)
        {
            bottomRight ??= bottomLeft + 1;
            topLeft ??= bottomLeft - 6;
        }

        if (bottomRight != null)
        {
            topRight ??= bottomRight - 6;
            bottomLeft ??= bottomRight - 1;
        }

        return (topLeft ?? 14, topRight ?? 15, bottomLeft ?? 20, bottomRight ?? 21);
    }
}
