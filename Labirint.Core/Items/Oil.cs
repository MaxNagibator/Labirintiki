namespace Labirint.Core.Items;

public class Oil : ScoreItem
{
    public Oil()
    {
        Name = "oil";
        DisplayName = "Нефть";

        CostPerItem = 100_000;

        SoundSettings = new SoundSettings(string.Empty, "/media/oil.mp3");
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) / (32 + 32);
    }

    protected override void AfterPlace(Position position, Labyrinth labyrinth)
    {
        base.AfterPlace(position, labyrinth);

        Position start = position - 1;
        Position end = position + 1;

        for (int x = start.X; x <= end.X; x++)
        {
            for (int y = start.Y; y <= end.Y; y++)
            {
                // Условие для создания формы в виде креста
                if ((x == start.X || x == end.X) && (y == start.Y || y == end.Y))
                {
                    continue;
                }

                labyrinth.CreateWall((x, y), Direction.All);
            }
        }
    }
}
