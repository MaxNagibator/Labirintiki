
using LabirintBlazorApp.Components;
using LabirintBlazorApp.Pages;
using MudBlazor;

namespace LabirintBlazorApp.Dto;

/// <summary>
/// Лабиринта.
/// </summary>
public class Labyrinth()
{
    /// <summary>
    /// Клеточки.
    /// </summary>
    private Tile[,] Tiles { get; set; }

    public Tile this[int x, int y] => Tiles[x, y];
    public Tile this[Position position] => Tiles[position.X, position.Y];

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int SandCount { get; private set; }

    public Position Player { get; private set; }

    public void Init(int width, int height, int _density, MazeSeed seeder)
    {
        Player = (0, 0);
        Width = width;
        Height = height;

        Tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = new Tile();
                Tiles[x, y] = tile;
                if (x == 0)
                {
                    tile.Walls = tile.Walls | Direction.Top;
                }
                if (x == width - 1)
                {
                    tile.Walls = tile.Walls | Direction.Bottom;
                }
                if (y == 0)
                {
                    tile.Walls = tile.Walls | Direction.Left;
                }
                if (y == height - 1)
                {
                    tile.Walls = tile.Walls | Direction.Right;
                }

                CreateWall(Direction.Top);
                CreateWall(Direction.Left);

                void CreateWall(Direction wallDirection)
                {
                    var a = seeder.Random.Next(0, 100);
                    if (a < _density)
                    {
                        tile.Walls = tile.Walls | wallDirection;
                        if (wallDirection == Direction.Top)
                        {
                            if (y > 0)
                            {
                                Tiles[x, y - 1].Walls |= Direction.Bottom;
                            }
                        }
                        if (wallDirection == Direction.Left)
                        {
                            if (x > 0)
                            {
                                Tiles[x - 1, y].Walls |= Direction.Right;
                            }
                        }
                    }
                }
            }
        }

        Tiles[width / 2, height - 1].IsExit = true;
        Tiles[width / 2, height - 1].Walls &= ~Direction.Bottom;

        SandCount = (width + height) / 2;
        for (int i = 1; i < SandCount; i++)
        {
            int x = seeder.Random.Next(0, width);
            int y = seeder.Random.Next(0, height);

            if (Tiles[x, y].HasSand == false)
            {
                Tiles[x, y].HasSand = true;
            }
            else
            {
                i--;
            }
        }
    }

    public void BreakWall(Direction direction)
    {
        if (direction == Direction.Left)
        {
            if (Player.X > 0)
            {
                Tiles[Player.X, Player.Y].Walls &= ~Direction.Left;
                Tiles[Player.X - 1, Player.Y].Walls &= ~Direction.Right;
            }
        }

        if (direction == Direction.Top)
        {
            if (Player.Y > 0)
            {
                Tiles[Player.X, Player.Y].Walls &= ~Direction.Top;
                Tiles[Player.X, Player.Y - 1].Walls &= ~Direction.Bottom;
            }
        }
        if (direction == Direction.Right)
        {
            if (Player.X < Width)
            {
                Tiles[Player.X, Player.Y].Walls &= ~Direction.Right;
                Tiles[Player.X + 1, Player.Y].Walls &= ~Direction.Left;
            }
        }
        if (direction == Direction.Bottom)
        {
            if (Player.Y < Height)
            {
                Tiles[Player.X, Player.Y].Walls &= ~Direction.Bottom;
                Tiles[Player.X, Player.Y + 1].Walls &= ~Direction.Top;
            }
        }
    }

    internal void Move(Direction direction)
    {
        if (Tiles[Player.X, Player.Y].Walls.HasFlag(direction))
        {
            switch (direction)
            {
                case Direction.Left:
                    Player += (-1, 0);
                    break;
                case Direction.Top:
                    Player += (0, -1);
                    break;
                case Direction.Right:
                    Player += (1, 0);
                    break;
                case Direction.Bottom:
                    Player += (0, 1);
                    break;
            }
        }
    }
}
