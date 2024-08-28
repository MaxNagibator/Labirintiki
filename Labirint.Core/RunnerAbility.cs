namespace Labirint.Core;

/// <summary>
///     Способность бегуна.
/// </summary>
public class RunnerAbility(Ability ability)
{
    private int? _lostCount = ability.MoveCount;

    public string DisplayName => ability.DisplayName;

    public string Icon => ability.Icon;

    public bool IsUnlimitedMoveCount => ability.IsUnlimitedMoveCount;

    public int LostCount => _lostCount ?? 0;

    public bool Active => _lostCount is not 0;

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
}
