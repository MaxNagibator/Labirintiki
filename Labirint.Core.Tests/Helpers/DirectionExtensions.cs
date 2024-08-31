namespace Labirint.Core.Tests.Helpers;

internal static class DirectionExtensions
{
    internal static Direction[] GetAll()
    {
        return [Direction.Left, Direction.Top, Direction.Right, Direction.Bottom];
    }

    internal static IEnumerable<(Direction direction, int count)> GetCombinedDirections(this IEnumerable<Direction> directions)
    {
        IEnumerable<Direction> enumerable = directions as Direction[] ?? directions.ToArray();
        int totalSubsets = 1 << enumerable.Count();

        for (int i = 1; i < totalSubsets; i++)
        {
            (Direction direction, int count) combination = new();

            yield return enumerable.Where((_, j) => (i & 1 << j) != 0)
                .Aggregate(combination, (current, direction) => (current.direction |= direction, current.count + 1));
        }
    }
}
