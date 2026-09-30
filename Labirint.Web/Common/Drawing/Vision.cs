namespace Labirint.Web.Common.Drawing;

public class Vision(int mazeWidth, int mazeHeight, int visionRange = 3)
{
    public int Range { get; } = visionRange;

    public Position Runner { get; private set; }
    public Position Origin { get; private set; }
    public Position Start { get; private set; }
    public Position Finish { get; private set; }

    public void SetPosition(Position position)
    {
        Runner = position;
        Origin = (position.X - Range, position.Y - Range);

        var startX = Math.Max(0, position.X - Range);
        var finishX = Math.Min(mazeWidth - 1, position.X + Range);

        var startY = Math.Max(0, position.Y - Range);
        var finishY = Math.Min(mazeHeight - 1, position.Y + Range);

        Start = (startX, startY);
        Finish = (finishX, finishY);
    }

    public Position GetDraw(Position position)
    {
        return position - Origin;
    }
}
