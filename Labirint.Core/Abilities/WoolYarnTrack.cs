namespace Labirint.Core.Abilities;

/// <summary>
///     След шерстяной нитки.
/// </summary>
public class WoolYarnTrack : Ability
{
    public override string Name => "WoolYarnTrack";

    public override string DisplayName => "След нити";

    public override int? MoveCount => 100;

    public override void Hit(Tile tile)
    {
        tile.TempWoolYarn = true;
    }
}
