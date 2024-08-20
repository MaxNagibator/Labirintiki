namespace Labirint.Core.Items;

public class Sand : ScoreItem
{
    public Sand()
    {
        Name = "sand";
        DisplayName = "Песочек";

        CostPerItem = 100;

        SoundSettings = new SoundSettings("", "score");
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) / 2;
    }

    public override WorldItem GetWorldItem(WorldItemParameters parameters)
    {
        return base.GetWorldItem(parameters) with
        {
            Alignment = Alignment.BottomCenter,
            Scale = parameters.Random.Random.Next(4, 7) / 10d
        };
    }

    protected override bool TryPickUp(WorldItem worldItem)
    {
        return Stack.TryAdd((int)Math.Floor(worldItem.Scale * 10 % 4 + 1));
    }
}
