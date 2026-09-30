namespace Labirint.Core.Items;

public class Oil : ScoreItem
{
    public override string Name => "oil";
    public override string DisplayName => "Нефть";

    public override string Description =>
        $"""
         Нефть - это настоящее сокровище в этом лабиринте, источник богатства и власти. 
         Представьте себе, как эти черные маслянистые лужицы разливаются по коридорам, создавая опасные ловушки для неосторожных путников. 
         Когда нефть начинает двигаться, словно живое существо, воздвигая вокруг себя ловушку из стен, словно защищая свое сокровище.
         ---
         Особенности нефти:
         - Каждая бочка нефти стоит целых {CostPerItem:N0} очков, так что вы можете стать настоящим нефтяным магнатом, если найдете достаточно ее в лабиринте!
         - Когда вы подбираете нефть, она издает характерный звук, похожий на бульканье.
         - Нефть не просто лежит на полу - она создает вокруг себя крестообразную структуру из стен, чтобы защитить свое сокровище от посягательств.
         ---
         Создатели лабиринта, должно быть, были одержимы идеей нефтяного богатства, когда разрабатывали этот предмет. 
         Так что не упустите свой шанс стать нефтяным магнатом и заработать целое состояние!
         """;

    public override int CostPerItem => 100_000;
    public override SoundSettings? SoundSettings { get; } = new("media/oil.mp3");

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) / (32 + 32);
    }

    protected override void AfterPlace(Position position, Labyrinth labyrinth)
    {
        base.AfterPlace(position, labyrinth);

        var start = position - 1;
        var end = position + 1;

        for (var x = start.X; x <= end.X; x++)
        {
            for (var y = start.Y; y <= end.Y; y++)
            {
                var isDiagonal = (x == start.X || x == end.X) && (y == start.Y || y == end.Y);

                if (isDiagonal)
                {
                    continue;
                }

                labyrinth.CreateWall((x, y), Direction.All);
            }
        }
    }
}
