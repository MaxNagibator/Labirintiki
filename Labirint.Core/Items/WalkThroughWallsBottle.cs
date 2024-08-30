using Labirint.Core.Abilities;
using Labirint.Core.TileFeatures;

namespace Labirint.Core.Items;

/// <summary>
///     Зелье прохождения сквозь стены.
/// </summary>
public class WalkThroughWallsBottle : Item
{
    public override string Name => "walk-trhought-walls-bottle";
    public override string DisplayName => "Сквозь стены";

    public override string Description =>
        """
        Зелье прохождения сквозь стены
        """;

    public override int DefaultCount => 0;
    public override int MaxCount => 1;

    public override bool UseAfterPickup => true;

    public override ControlSettings? ControlSettings { get; } = new(Key.KeyT);
    public override SoundSettings? SoundSettings { get; } = null;

    public virtual WorldItem GetWorldItem(WorldItemParameters parameters)
    {
        return new WorldItem(this, Image, Alignment.Center, 0.5)
        {
            AfterPlace = AfterPlace
        };
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400 / 4;
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        // todo сделать способность продлеваемой или перезапуск до максимума: опционально
        WalkThroughWallsAbility track = new();
        labyrinth.Runner.AddAbility(track);
    }
}
