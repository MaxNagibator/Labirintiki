using Labirint.Core.Abilities;

namespace Labirint.Core.Items;

/// <summary>
/// Шерстяная нить.
/// </summary>
public class WoolYarn : Item
{
    public WoolYarn()
    {
        Name = "WoolYarn";
        DisplayName = "Нить";

        Stack = new LimitedItemStack(this, 0, 1);

        ControlSettings = new ControlSettings(Key.KeyB, Key.ControlLeft);
        SoundSettings = new SoundSettings("bomb", "score");
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) / (32 + 32);
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        labyrinth.Runner.AddAbility(new WoolYarnTrack());
    }
}
