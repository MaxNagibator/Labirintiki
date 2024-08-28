using Labirint.Core.Common;
using Labirint.Core.Extensions;
using Labirint.Core.TileFeatures;
using System.Net;

namespace Labirint.Core.Abilities;

/// <summary>
///     След шерстяной нитки.
/// </summary>
public class WoolYarnAbility : Ability
{
    public override string Name => "WoolYarnTrack";

    public override string DisplayName => "След нити";

    public override int? MoveCount => 100;

    public override void Hit(Tile tile, Direction direction)
    {
        Position prevTilePostion = tile.Labyrinth.Runner.Position - direction;
        Tile prevTile = tile.Labyrinth[prevTilePostion];

        prevTile.AddFeature(new WoolYarnFeature(direction));
        tile.AddFeature(new WoolYarnFeature(direction.GetOppositeDirection()));
    }
}
