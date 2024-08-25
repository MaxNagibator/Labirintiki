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
        _labyrinth = labyrinth;

        _abilities = [];
        Inventory = new Inventory();

        // TODO Подумать над отпиской
        Inventory.ScoreIncrease += (_, amount) => Score += amount;
    }

    public Position Position { get; private set; }
    public int Score { get; private set; }

    public Inventory Inventory { get; }

    public IReadOnlyList<RunnerAbility> Abilities => _abilities;

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

    public void UseItem(Item item, Direction? direction)
    {
        Inventory.Use(item, Position, direction, _labyrinth);
    }
}
