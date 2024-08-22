namespace Labirint.Core.Items;

/// <summary>
///     Шерстяная нить.
/// </summary>
public class WoolYarn : Item
{
    public WoolYarn()
    {
        Name = "wool-yarn";
        DisplayName = "Нить";

        Stack = new LimitedItemStack(this, 0, 1);

        ControlSettings = new ControlSettings(Key.KeyY, Key.KeyY);
        SoundSettings = new SoundSettings("/media/wool-yarn.mp3", "/media/yarn.mp3");
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
