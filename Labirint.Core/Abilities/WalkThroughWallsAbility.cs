using Labirint.Core.Abilities.Prolongations;

namespace Labirint.Core.Abilities;

/// <summary>
///     Прохождение сквозь стены.
/// </summary>
public class WalkThroughWallsAbility : Ability
{
    public override string Name => "walk-through-walls";

    public override string DisplayName => "Сквозь стены";

    public override int? MoveCount => 5;

    public override bool IsIgnoreWalls => true;

    public override AbilityProlongation Prolongation { get; } = new ResetProlongation();
}
