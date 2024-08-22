namespace Labirint.Core;

/// <summary>
///     Способность бегуна.
/// </summary>
public class RunnerAbility(Ability ability)
{
    private int? _lostCount = ability.MoveCount;

    public bool Active => _lostCount is not 0;

    public void Hit()
    {
        if (Active == false)
        {
            return;
        }

        if (_lostCount != null)
        {
            _lostCount--;
        }

        ability.Hit();
    }
}
