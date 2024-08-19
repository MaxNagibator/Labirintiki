namespace Labirint.Core.Items;

public class Hammer : Item
{
    public Hammer()
    {
        Name = "hammer";
        DisplayName = "Молоток";

        Stack = new LimitedItemStack(this, 6, 6);

        ControlSettings = new ControlSettings(Key.KeyA, Key.Space, true);
        SoundSettings = new SoundSettings("molot", "score");
    }

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
