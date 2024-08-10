namespace LabirintBlazorApp.Dto;

public class Vision(int mazeWidth, int mazeHeight, int visionRangeBase = 3)
{
    public int Range { get; } = visionRangeBase * 2;

    public Position Player { get; private set; }
    public Position Start { get; private set; }
    public Position Finish { get; private set; }

    public void SetPosition(Position position)
    {
        Player = position;

        int startX = Math.Max(1, position.X - Range);
        int finishX = Math.Min(mazeHeight - 1, position.X + Range + 2);

        int startY = Math.Max(1, position.Y - Range);
        int finishY = Math.Min(mazeWidth - 1, position.Y + Range + 2);

        Start = (startX, startY);
        Finish = (finishX, finishY);
    }

    public Position GetDraw(Position position)
    {
        return (position.X - Start.X + 1, position.Y - Start.Y + 1);
    }
}
