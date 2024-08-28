using Labirint.Core.Abilities;

namespace Labirint.Core.Items;

/// <summary>
///     Шерстяная нить.
/// </summary>
public class WoolYarn : Item
{
    public override string Name => "wool-yarn";
    public override string DisplayName => "Нить";

    public override string Description =>
        """
        Нить - это ваш верный спутник в этом запутанном лабиринте, ваша ниточка Ариадны, ведущая вас сквозь этот лабиринтный хаос. 
        Представьте себе, как вы разматываете этот клубок шерстяной пряжи, оставляя за собой яркий кровавый след, словно коварный паук, плетущий свою паутину. 
        Когда вы активируете нить, она начинает светиться и пульсировать, словно сердце лабиринта, отмечая ваш маршрут.
        ---
        Особенности нити:
        - Нить настолько бесценна, что её можно найти только в самых темных уголках. Используйте её с умом, чтобы не заблудиться в этом лабиринте.
        - Каждый раз, когда вы используете нить, она издает характерный звук, похожий на шуршание шерсти.
        - Нить оставляет за вами яркий красный след, чтобы вы могли отслеживать свой путь и не заблудиться.
        ---
        Создатели лабиринта, должно быть, были вдохновлены древнегреческими мифами, когда разрабатывали этот предмет. 
        Так что не упустите свой шанс стать современным Тесеем и выбраться из этого лабиринта с помощью своей верной нити!
        """;

    public override int DefaultCount => 0;
    public override int MaxCount => 1;

    public override ControlSettings? ControlSettings { get; } = new(Key.KeyY);
    public override SoundSettings? SoundSettings { get; } = new("/media/yarn.mp3", "/media/wool-yarn.mp3");

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return 10; // (width + height) / (32 + 32);
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        WoolYarnAbility track = new();
        labyrinth.Runner.AddAbility(track);
        track.Hit(labyrinth[position]);
    }
}
