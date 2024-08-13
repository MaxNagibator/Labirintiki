namespace Labirint.Core.Items;

public class Hammer : Item
{
    public Hammer()
    {
        Name = "hammer";
        DisplayName = "Молоток";
        Count = 6;
        MaxCount = 6;
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400;
    }
}
