namespace Labirint.Web.Parameters;

public record MazeRenderParameters(Labyrinth Maze, int BoxSize, int WallWidth, Vision Vision)
{
    public int RenderRange { get; } = Vision.Range * 2 * BoxSize + BoxSize + WallWidth;
}
