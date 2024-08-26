namespace Labirint.Core.Items;

public class Bomb : Item
{
    public override string Name => "bomb";
    public override string DisplayName => "Бомба";

    public override int DefaultCount => 2;
    public override int MaxCount => 2;

    public override ControlSettings? ControlSettings { get; } = new(Key.KeyB, Key.ControlLeft);
    public override SoundSettings? SoundSettings { get; } = new("/media/bomb.mp3", "bomb");

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400 / 2;
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        labyrinth.BreakWall(position, Direction.All);
    }
}
