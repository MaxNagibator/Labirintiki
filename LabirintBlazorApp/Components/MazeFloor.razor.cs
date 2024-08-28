namespace LabirintBlazorApp.Components;

public partial class MazeFloor : MazeComponent
{
    protected override string CanvasId => "mazeFloorCanvas";

    protected override string StrokeStyle => "black";

    protected override void DrawInner(int x, int y, DrawSequence sequence)
    {
        Position topLeft = Vision.GetDraw((x, y)) * BoxSize;

        sequence.DrawRect(topLeft.X, topLeft.Y, BoxSize + WallWidth, BoxSize + WallWidth);

        int tileSize = BoxSize / 2;

        int[,] tile = GetTile(x, y);

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int left = topLeft.X + i * tileSize;
                int top = topLeft.Y + j * tileSize;

                sequence.DrawSprite("/images/tiles/floor.png", tile[i, j] / 6, tile[i, j] % 6, left, top, tileSize);
            }
        }
    }

    // https://i.imgur.com/WL6Nt13.png
    private int[,] GetTile(int x, int y)
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

        int[,] tile = new int[2, 2];

        tile[0, 0] = topLeft ?? 14;
        tile[1, 0] = topRight ?? 15;
        tile[0, 1] = bottomLeft ?? 20;
        tile[1, 1] = bottomRight ?? 21;

        return tile;
    }
}
