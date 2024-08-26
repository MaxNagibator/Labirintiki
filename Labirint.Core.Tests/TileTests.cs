namespace Labirint.Core.Tests;

[TestFixture]
public class TileTests
{
    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            Direction[] directions = [Direction.None, Direction.Left, Direction.Top, Direction.Right, Direction.Bottom];

            IEnumerable<(Direction direction, int count)> combinedDirections = GetCombinedDirections(directions.Where(direction => direction != Direction.None).ToArray());

            foreach ((Direction direction, int count) in combinedDirections)
            {
                foreach (Direction directionToAdd in directions)
                {
                    bool expected = direction != Direction.All
                                    && direction != Direction.None
                                    && direction != directionToAdd
                                    && direction.HasFlag(directionToAdd) == false
                                    && count < 3;

                    yield return new TestCaseData(direction, directionToAdd, expected);
                }
            }
        }
    }

    private static IEnumerable<(Direction direction, int count)> GetCombinedDirections(Direction[] directions)
    {
        int totalSubsets = 1 << directions.Length;

        for (int i = 1; i < totalSubsets; i++)
        {
            (Direction direction, int count) combination = new();

            for (int j = 0; j < directions.Length; j++)
            {
                if ((i & 1 << j) != 0)
                {
                    combination = (combination.direction |= directions[j], combination.count + 1);
                }
            }

            yield return combination;
        }
    }

    [TestCaseSource(nameof(TestCases))]
    public void CanAddWallTest(Direction existingWalls, Direction directionToAdd, bool expectedResult)
    {
        Tile tile = new(default)
        {
            Walls = existingWalls
        };

        bool result = tile.CanAddWall(directionToAdd);

        Assert.That(result, Is.EqualTo(expectedResult));
    }
}
