namespace Labirint.Core.Items;

public class Hammer : Item
{
    public override string Name => "hammer";
    public override string DisplayName => "Молоток";

    public override string Description => $"""
                                           Молоток - это ваш верный спутник в этом запутанном лабиринте, ваш личный инструмент для разрушения. 
                                           Представьте себе, как вы с грохотом и лязгом пробиваете себе путь сквозь стены, словно строитель-разрушитель, 
                                           оставляя за собой груды обломков. Этот массивный, тяжелый молоток кажется неуклюжим, 
                                           но в ваших умелых руках он становится оружием, способным сокрушить любое препятствие.
                                           ---
                                           Особенности молотка:
                                           - Вы получаете целых {DefaultCount} штук, чтобы устраивать настоящий разгром в лабиринте!
                                           - Необходимо сначала нажать клавишу активации, а затем шагнуть, чтобы обрушить свой гнев на очередную стену.
                                           - Каждый раз, когда вы используете молоток, он издает громкий, удовлетворяющий звук "бам!", 
                                           который, говорят, можно услышать даже в самых дальних уголках лабиринта.
                                           ---
                                           Создатели лабиринта, должно быть, предвидели, что вам понадобится мощное оружие для борьбы с этими коварными стенами. 
                                           Так что не стесняйтесь использовать молоток, чтобы проложить себе путь к победе!
                                           """;

    public override int DefaultCount => 6;
    public override int MaxCount => 6;

    public override ControlSettings? ControlSettings { get; } = new(Key.KeyA, Key.Space, true);
    public override SoundSettings? SoundSettings { get; } = new("/media/hammer.mp3", "molot");

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400;
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        if (direction == null)
        {
            return;
        }

        labyrinth.BreakWall(position, direction.Value);
    }
}
