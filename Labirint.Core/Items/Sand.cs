using Labirint.Core.TileFeatures;

namespace Labirint.Core.Items;

public class Sand : ScoreItem
{
    private const int MinSize = 6;
    private const int MaxSize = 9;

    public override string Name => "sand";
    public override string DisplayName => "Песочек";

    public override string Description =>
        $"""
         Песочек - это настоящая находка в этом лабиринте, источник развлечения и веселья. 
         Представьте себе, как эти крошечные золотистые зернышки рассыпаются по коридорам, создавая причудливые узоры и ловушки для неосторожных путников. 
         Когда на вашем пути появляется песочек, он начинает собираться в причудливые кучки и холмики, словно ведомый древней стихией, играющей в свои игры.
         ---
         Особенности песочка:
         - Каждая горстка песочка стоит целых {CostPerItem} очков, так что вы можете стать настоящим песочным королём, если найдете достаточно его в лабиринте!
         - Количество песочка в каждой горстке варьируется от {1} до {MaxSize % MinSize + 1} зернышек, так что вы никогда не знаете, что вас ждет.
         - Когда вы собираете песочек, он издает характерный звук, похожий на заветное повышение очков.
         ---
         Создатели лабиринта, должно быть, были одержимы идеей песочной империи, когда разрабатывали этот предмет. 
         Так что не упустите свой шанс построить неприступную песчаную крепость и слепить себе волшебных прислужниц!
         """;

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
