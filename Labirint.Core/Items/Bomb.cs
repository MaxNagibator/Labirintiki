namespace Labirint.Core.Items;

public class Bomb : Item
{
    public Bomb()
    {
        Name = "bomb";
        DisplayName = "Бомба";
        Count = 2;
        MaxCount = 2;
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400 / 2;
    }
}
