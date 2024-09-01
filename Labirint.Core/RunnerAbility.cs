namespace Labirint.Core;

/// <summary>
///     Способность бегуна.
/// </summary>
public class RunnerAbility(Ability ability)
{
    private int? _lostCount = ability.MoveCount;

    public Ability Properties => ability;

    // todo убрать проперти, которые можно получить из ability
    public string DisplayName => ability.DisplayName;

    public string Icon => ability.Icon;

    public bool IsUnlimitedMoveCount => ability.IsUnlimitedMoveCount;

    public int LostCount => _lostCount ?? 0;

    public bool Active => _lostCount is not 0;

    public AbilityProlongation Prolongation => ability.Prolongation;

    public bool ContainsAbility(Func<Ability, bool> func)
    {
        return Active && func(ability);
    }

    public void Hit(Tile tile, Direction direction)
    {
        if (Active == false)
        {
            return;
        }

        if (_lostCount != null)
        {
            _lostCount--;
        }

        ability.Hit(tile, direction);
    }

    public void Prolong()
    {
        if (_lostCount == null || ability.MoveCount == null || ability.IsUnlimitedMoveCount)
        {
            return;
        }

        ability.Prolongation.Prolong(ref _lostCount, ability.MoveCount.Value);
    }
}
