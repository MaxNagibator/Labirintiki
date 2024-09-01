namespace Labirint.Core;

/// <summary>
///     Бегущий по лабиринту.
/// </summary>
public class Runner : IDisposable
{
    private readonly List<RunnerAbility> _abilities;
    private readonly Labyrinth _labyrinth;

    public Runner(Position position, Labyrinth labyrinth, Inventory inventory)
    {
        Position = position;
        _labyrinth = labyrinth;
        Inventory = inventory;

        _abilities = [];
        Inventory.ScoreIncreased += OnScoreIncreased;
    }

    public Position Position { get; private set; }
    public int Score { get; private set; }

    public Inventory Inventory { get; }

    public IReadOnlyList<RunnerAbility> Abilities => _abilities;

    /// <summary>
    ///     Последний ход в лабиринте.
    /// </summary>
    public Direction LastDirection { get; private set; }

    public void AddAbility(Ability ability)
    {
        RunnerAbility? currentAbility = _abilities.SingleOrDefault(runnerAbility => runnerAbility.Properties.Name == ability.Name);

        if (currentAbility != null)
        {
            currentAbility.Prolong();
        }
        else
        {
            _abilities.Add(new RunnerAbility(ability));
        }
    }

    public bool Move(Direction direction)
    {
        // TODO Можно выйти за границы лабиринта
        if (direction == Direction.All
            || _labyrinth[Position].ContainsWall(direction)
            && ContainsActiveAbility(ability => ability.IsIgnoreWalls) == false)
        {
            return false;
        }

        LastDirection = direction;
        Position += direction;

        foreach (RunnerAbility ability in _abilities)
        {
            ability.Hit(_labyrinth[Position], direction);
        }

        return true;
    }

    public void UseItem(Item item, Direction? direction)
    {
        Inventory.Use(item, Position, direction, _labyrinth);
    }

    public void Dispose()
    {
        Inventory.ScoreIncreased -= OnScoreIncreased;

        GC.SuppressFinalize(this);
    }

    public void Reset()
    {
        Position = (0, 0);
        Score = 0;
        LastDirection = Direction.None;

        Inventory.Clear();
        _abilities.Clear();
    }

    private void OnScoreIncreased(object? sender, int amount)
    {
        Score += amount;
    }

    private bool ContainsActiveAbility(Func<Ability, bool> predicate)
    {
        return _abilities.Any(ability => ability.Active && predicate(ability.Properties));
    }
}
