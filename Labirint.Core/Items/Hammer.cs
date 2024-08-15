namespace Labirint.Core.Items;

public class Hammer : Item
{
    public Hammer()
    {
        Name = "hammer";
        DisplayName = "Молоток";
        DefaultCount = 6;
        MaxCount = 6;

        SoundType = "molot";

        ControlSettings = new ControlSettings(Key.KeyA, true);
    }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return (width + height) * density / 400;
    }

    public override bool TryUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
        if (base.TryUse(position, direction, labyrinth) == false)
        {
            return false;
        }

        if (direction == null)
        {
            return false;
        }

        labyrinth.BreakWall(position, direction.Value);
        return true;
    }
}
