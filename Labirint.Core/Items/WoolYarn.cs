namespace Labirint.Core.Items;

/// <summary>
///     Шерстяная нить.
/// </summary>
public class WoolYarn : Item
{
    public WoolYarn()
    {
        Name = "WoolYarn";
        DisplayName = "Нить";

        Stack = new LimitedItemStack(this, 0, 1);

        ControlSettings = new ControlSettings(Key.KeyY, Key.KeyY);
        SoundSettings = new SoundSettings("bomb", "score");
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) / (32 + 32);
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        var track = new WoolYarnTrack();
        labyrinth.Runner.AddAbility(track);
        track.Hit(labyrinth[position]);
    }
}
