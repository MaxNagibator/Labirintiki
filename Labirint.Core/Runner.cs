using Labirint.Core.Abilities;
using Labirint.Core.Common;

namespace Labirint.Core;

/// <summary>
///     Бегущий по лабиринту.
/// </summary>
public class Runner
{
    public Runner(Position position)
    {
        Position = position;
    }

    public Position Position { get; set; }

    public List<RunnerAbility> Abilities { get; set; }

    public void AddAbility(Ability ability)
    {
        Abilities.Add(new RunnerAbility(ability));
    }

    public void Move(Position position)
    {
        Position += position;
        foreach (var ability in Abilities)
        {
            ability.Hit();
        }
    }
}
