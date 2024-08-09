namespace LabirintBlazorApp.Dto;

public class Vision(int mazeWidth, int mazeHeight, int visionRangeBase = 3)
{
    public int Range { get; } = visionRangeBase * 2;

    public Position Player { get; private set; }
    public Position Start { get; private set; }
    public Position Finish { get; private set; }

    public void SetPosition(int x, int y)
    {
        Player = (x, y);

        int startX = Math.Max(1, x - Range);
        int finishX = Math.Min(mazeHeight - 1, x + Range + 2);

        int startY = Math.Max(1, y - Range);
        int finishY = Math.Min(mazeWidth - 1, y + Range + 2);

        Start = (startX, startY);
        Finish = (finishX, finishY);
    }

    public Position GetDraw(int x, int y)
    {
        return (x - Start.X + 1, y - Start.Y + 1);
    }
}
