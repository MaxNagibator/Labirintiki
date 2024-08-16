namespace Labirint.Core.Items;

public class Sand : ScoreItem
{
    // TODO Подумать и возможно добавить сидирование
    private readonly Random _random = new();

    public Sand()
    {
        Name = "sand";
        DisplayName = "Песочек";

        CostPerItem = 100;

        SoundSettings = new SoundSettings("", "score");
    }

    protected override bool TryPickUp(WorldItem worldItem)
    {
        return Stack.TryAdd((int)Math.Floor(worldItem.Scale * 10 % 4 + 1));
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
            Scale = _random.Next(4, 7) / 10d
        };
    }
}
