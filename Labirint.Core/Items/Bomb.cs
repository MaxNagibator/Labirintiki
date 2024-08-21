namespace Labirint.Core.Items;

public class Bomb : Item
{
    public Bomb()
    {
        Name = "bomb";
        DisplayName = "Бомба";

        Stack = new LimitedItemStack(this, 2, 2);

        ControlSettings = new ControlSettings(Key.KeyB, Key.ControlLeft);
        SoundSettings = new SoundSettings("bomb", "score");
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400 / 2;
    }

    protected override void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        labyrinth.BreakWall(position, Direction.All);
    }
}
