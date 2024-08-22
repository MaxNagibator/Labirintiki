namespace Labirint.Core;

/// <summary>
///     Бегущий по лабиринту.
/// </summary>
public class Runner
{
    private readonly List<RunnerAbility> _abilities;

    public Runner(Position position)
    {
        Position = position;
        _abilities = [];
    }

    public Position Position { get; private set; }

    public IEnumerable<RunnerAbility> Abilities => _abilities;

    public void AddAbility(Ability ability)
    {
        _abilities.Add(new RunnerAbility(ability));
    }

    public void Move(Direction direction)
    {
        Position += direction;

        foreach (RunnerAbility ability in _abilities)
        {
            ability.Hit();
        }
    }
}
