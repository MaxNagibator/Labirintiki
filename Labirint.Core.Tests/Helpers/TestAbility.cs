namespace Labirint.Core.Tests.Helpers;

internal class TestAbility(int? moveCount = null) : Ability
{
    public override string Name => "TestAbility";

    public override int? MoveCount { get; } = moveCount;

    public int HitCount { get; private set; }

    public override void Hit()
    {
        HitCount++;
    }
}
