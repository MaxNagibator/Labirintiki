using LabirintBlazorApp.Common.Drawing;

namespace LabirintBlazorApp.Parameters;

public record MazeRenderParameter(Labyrinth Maze, int BoxSize, int WallWidth, Vision Vision);
