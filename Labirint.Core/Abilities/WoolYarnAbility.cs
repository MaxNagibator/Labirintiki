using Labirint.Core.Abilities.Prolongations;
using Labirint.Core.Extensions;
using Labirint.Core.TileFeatures;

namespace Labirint.Core.Abilities;

/// <summary>
/// След шерстяной нитки.
/// </summary>
public class WoolYarnAbility : Ability
{
    public override string Name => "wool-yarn-track";

    public override string DisplayName => "След нити";

    public override int? MoveCount => 100;

    public override AbilityProlongation Prolongation { get; } = new SumProlongation();

    public override void Hit(Tile tile, Direction direction)
    {
        var prevTilePosition = tile.Labyrinth.Runner.Position - direction;
        var prevTile = tile.Labyrinth[prevTilePosition];

        prevTile.AddFeature(new WoolYarnFeature(direction));
        tile.AddFeature(new WoolYarnFeature(direction.GetOppositeDirection()));
    }
}
