using Labirint.Core.Abilities;

namespace Labirint.Core.Items;

/// <summary>
///     Шерстяная нить.
/// </summary>
public class WoolYarn : Item
{
    public override string Name => "wool-yarn";
    public override string DisplayName => "Нить";

    public override int DefaultCount => 0;
    public override int MaxCount => 1;

    public override ControlSettings? ControlSettings { get; } = new(Key.KeyY);
    public override SoundSettings? SoundSettings { get; } = new("/media/yarn.mp3", "/media/wool-yarn.mp3");

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return 10; // (width + height) / (32 + 32);
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        WoolYarnAbility track = new();
        labyrinth.Runner.AddAbility(track);
        track.Hit(labyrinth[position]);
    }
}
