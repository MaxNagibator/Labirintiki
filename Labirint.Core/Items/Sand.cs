using Labirint.Core.TileFeatures;

namespace Labirint.Core.Items;

public class Sand : ScoreItem
{
    private const int MinSize = 6;
    private const int MaxSize = 9;

    public override string Name => "sand";
    public override string DisplayName => "Песочек";

    public override int CostPerItem => 100;

    public override SoundSettings? SoundSettings { get; } = new("score");

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) / 2;
    }

    public override WorldItem GetWorldItem(WorldItemParameters parameters)
    {
        WorldItem item = base.GetWorldItem(parameters);

        int count = parameters.Random.Generator.Next(MinSize, MaxSize + 1);

        return new WorldItem(this, Image, Alignment.BottomCenter, count / 10d)
        {
            AfterPlace = item.AfterPlace,
            PickUpCount = count % MinSize + 1
        };
    }
}
