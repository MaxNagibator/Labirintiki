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
    /// Последний ход в лабиринте.
    /// </summary>
    public Direction LastDirection { get; private set; }

    public void AddAbility(Ability ability)
    {
        _abilities.Add(new RunnerAbility(ability));
    }

    public void Move(Direction direction)
    {
        LastDirection = direction;
        Position += direction;

        foreach (RunnerAbility ability in _abilities)
        {
            ability.Hit(_labyrinth[Position], direction);
        }
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
        Inventory.Clear();
        _abilities.Clear();
    }

    private void OnScoreIncreased(object? sender, int amount)
    {
        Score += amount;
    }
}
