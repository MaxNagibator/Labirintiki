namespace Labirint.Core.Abilities;

/// <summary>
///     След шерстяной нитки.
/// </summary>
public class WoolYarnTrack : Ability
{
    public override string Name => "След нити";

    public override int? MoveCount => 100;

    public override void Hit()
    {
        throw new NotImplementedException();
    }
}
