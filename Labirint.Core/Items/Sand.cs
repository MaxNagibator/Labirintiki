namespace Labirint.Core.Items;

public class Sand : Item
{
    public Sand()
    {
        Name = "sand";
        DisplayName = "Песок";
        MaxCount = 1000;
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) / 2;
    }
}
