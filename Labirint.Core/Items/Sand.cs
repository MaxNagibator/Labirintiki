namespace Labirint.Core.Items;

public class Sand : Item
{
    public Sand()
    {
        Name = "sand";
        DisplayName = "Песок";
        MaxCount = 1000;

        SoundType = "score";
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) / 2;
    }

    public override WorldItem GetWorldItem()
    {
        return base.GetWorldItem() with
        {
            Alignment = Alignment.BottomCenter,
            Scale = 0.5
        };
    }
}
