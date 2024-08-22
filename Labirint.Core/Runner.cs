namespace Labirint.Core;

/// <summary>
///     Бегущий по лабиринту.
/// </summary>
public class Runner
{
    private readonly List<RunnerAbility> _abilities;
    private readonly Labyrinth _labyrinth;

    public Runner(Position position, Labyrinth labyrinth)
    {
        Position = position;
        _abilities = [];
        _labyrinth = labyrinth;
    }

    public Position Position { get; private set; }

    public List<RunnerAbility> Abilities => _abilities;

    public void AddAbility(Ability ability)
    {
        _abilities.Add(new RunnerAbility(ability));
    }

    public void Move(Direction direction)
    {
        Position += direction;

        foreach (RunnerAbility ability in _abilities)
        {
            ability.Hit(_labyrinth[Position]);
        }
    }
}
