using Labirint.Core.TileFeatures;

namespace Labirint.Core.Tests.Helpers;

internal static class LabyrinthExtensions
{
    internal static IEnumerable<Tile> Enumerate(this Labyrinth labyrinth)
    {
        for (var x = 0; x < labyrinth.Width; x++)
        {
            for (var y = 0; y < labyrinth.Height; y++)
            {
                yield return labyrinth[x, y];
            }
        }
    }

    internal static int GetInMazeCount(this Labyrinth labyrinth, Item item)
    {
        return labyrinth.Enumerate()
            .SelectMany(tile => tile.Features ?? [])
            .OfType<WorldItem>()
            .Count(worldItem => worldItem.Item == item);
    }
}
