namespace Labirint.Core.Items;

public class Bomb : Item
{
    public override string Name => "bomb";
    public override string DisplayName => "Бомба";

    public override string Description =>
        $"""
         Бомба - это ваш верная спутница (главная цаца) в этом запутанном лабиринте. 
         Она не просто взрывается, а в пляске разрушения сокрушает стены и открывает новые пути. 
         Представьте себе, как эта маленькая сфера грациозно порхает по коридорам, оставляя за собой шлейф из обломков и пыли. 
         Когда вы активируете её, она начинает подпрыгивать и вращаться, словно одержимая страстью разрушения, пока, наконец, не взрывается в эффектном фейерверке.
         ---
         Бомба обладает уникальными свойствами:
         - Она всегда приходит в комплекте из {DefaultCount} штук, чтобы вы могли устраивать двойные взрывы и удивлять своих соперников.
         - Каждый раз, когда вы используете бомбу, она издает характерный звук, который, говорят, можно услышать даже в самых дальних уголках лабиринта.
         ---
         Говорят, что создатели лабиринта специально разработали эту бомбу, чтобы добавить немного драматизма и веселья в ваше путешествие. 
         Так что не стесняйтесь использовать её, чтобы проложить себе путь к победе!
         """;

    public override int DefaultCount => 2;
    public override int MaxCount => 2;

    public override ControlSettings? ControlSettings { get; } = new(Key.KeyB, Key.ControlLeft);
    public override SoundSettings? SoundSettings { get; } = new("media/bomb.mp3", "bomb");

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400 / 2;
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        labyrinth.BreakWall(position, Direction.All);
    }
}
