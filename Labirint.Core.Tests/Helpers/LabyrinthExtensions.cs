namespace Labirint.Core.Tests.Helpers;

internal static class LabyrinthExtensions
{
    internal static IEnumerable<Tile> Enumerate(this Labyrinth labyrinth)
    {
        for (int x = 0; x < labyrinth.Width; x++)
        {
            for (int y = 0; y < labyrinth.Height; y++)
            {
                yield return labyrinth[x, y];
            }
        }
    }
}
