using Labirint.Core.Abilities;
using Labirint.Core.TileFeatures;

namespace Labirint.Core.Items;

/// <summary>
///     Зелье прохождения сквозь стены.
/// </summary>
public class WalkThroughWallsBottle : Item
{
    public override string Name => "walk-through-walls-bottle";
    public override string DisplayName => "Сквозь стены";

    public override string Description =>
        """
        Зелье прохождения сквозь стены
        """;

    public override int DefaultCount => 0;
    public override int MaxCount => 1;

    public override bool UseAfterPickup => true;

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400 / 4;
    }

    protected override WorldItem GetWorldItem(WorldItemParameters parameters)
    {
        return new WorldItem(this, Image, Alignment.BottomCenter, 0.5);
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        WalkThroughWallsAbility track = new();
        labyrinth.Runner.AddAbility(track);
    }
}
