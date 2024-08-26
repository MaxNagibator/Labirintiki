namespace Labirint.Core.Items;

public class Hammer : Item
{
    public override string Name => "hammer";
    public override string DisplayName => "Молоток";

    public override int DefaultCount => 6;
    public override int MaxCount => 6;

    public override ControlSettings? ControlSettings { get; } = new(Key.KeyA, Key.Space, true);
    public override SoundSettings? SoundSettings { get; } = new("/media/hammer.mp3", "molot");

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400;
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        if (direction == null)
        {
            return;
        }

        labyrinth.BreakWall(position, direction.Value);
    }
}
