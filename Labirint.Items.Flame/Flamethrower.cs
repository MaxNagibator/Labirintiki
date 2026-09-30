namespace Labirint.Items.Flame;

public class Flamethrower : Item
{
    private const int Range = 3;

    public override string Name => "flamethrower";
    public override string DisplayName => "Огнемёт";

    public override string Description =>
        $"""
         Огнемёт - это тот случай, когда лабиринт спрашивает вас про терпение, а вы отвечаете ему про давление в баллоне.
         Струя уходит вперёд и не спрашивает, сколько там стен: она проходит их насквозь, оставляя за собой ровный коридор и запах гари.
         Держите его крепче, иначе коридор получится не там, где вы задумали.
         ---
         Что нужно знать про огнемёт:
         - Одна струя сносит до {Range} стен подряд в выбранном направлении.
         - Сначала нажмите клавишу активации, потом шагните в сторону будущего коридора.
         - У края лабиринта струя выдыхается: породу она не берёт.
         ---
         Создатели лабиринта такого не предусматривали, но и запретить не догадались.
         """;

    public override int DefaultCount => 0;
    public override int MaxCount => 2;

    public override string Kind => "Ультимативное снаряжение";

    public override IReadOnlyList<ItemStat> Stats => [new("Дальность", Range.ToString(), " стены подряд")];

    public override string ResourceRoot => "_content/Labirint.Items.Flame/images/items";

    public override ControlSettings? ControlSettings { get; } = new(Key.KeyF, MoveRequired: true);

    public override SoundSettings? SoundSettings { get; } =
        new("_content/Labirint.Items.Flame/media/flamethrower.mp3", "_content/Labirint.Items.Flame/media/flamethrower.mp3");

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400 / 3;
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        if (direction == null || direction == Direction.All)
        {
            return;
        }

        var current = position;

        for (var step = 0; step < Range; step++)
        {
            var next = current + direction.Value;

            if (labyrinth.Contains(next) == false)
            {
                return;
            }

            labyrinth.BreakWall(current, direction.Value);
            current = next;
        }
    }
}
