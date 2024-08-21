using Labirint.Core.Abilities;

namespace Labirint.Core;

/// <summary>
///    Способность бегуна.
/// </summary>
public class RunnerAbility
{
    private Ability _ability;
    private int? _lostCount;
    public bool Active { get; private set; }

    public RunnerAbility(Ability ability)
    {
        _ability = ability;
        _lostCount = ability.IsUnlimitedMoveCount ? null : ability.MoveCount;
    }

    public void Hit()
    {
        if (!Active)
        {
            return;
        }

        if (_lostCount != null)
        {
            _lostCount--;
            if (_lostCount == 0)
            {
                Active = false;
            }
        }

        _ability.Hit();
    }
}
