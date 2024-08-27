using Labirint.Core.TileFeatures;

namespace Labirint.Core.Items;

public class Sand : ScoreItem
{
    private const int MinSize = 6;
    private const int MaxSize = 9;

    public override string Name => "sand";
    public override string DisplayName => "Песочек";

    public override string Description => $"""
                                           Песочек - это настоящее сокровище в этом лабиринте, источник развлечения и веселья. 
                                           Представьте себе, как эти крошечные золотистые зернышки рассыпаются по коридорам, 
                                           создавая причудливые узоры и ловушки для неосторожных путников. 
                                           Когда вы размещаете песочек, он начинает собираться в причудливые кучки и холмики, 
                                           словно живое существо, играющее в свои игры.
                                           ---
                                           Особенности песочка:
                                           - Каждая горстка песочка стоит целых {CostPerItem} очков, так что вы можете стать настоящим песочным магнатом, 
                                           если найдете достаточно его в лабиринте!
                                           - Количество песочка в каждой горстке варьируется от 6 до 9 зернышек, так что вы никогда не знаете, что вас ждет.
                                           - Когда вы собираете песочек, он издает характерный звук, похожий на шуршание, который, говорят, 
                                           можно услышать даже в самых дальних уголках лабиринта.
                                           ---
                                           Создатели лабиринта, должно быть, были одержимы идеей песочного веселья, когда разрабатывали этот предмет. 
                                           Так что не упустите свой шанс стать песочным магнатом и заработать целое состояние!
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
