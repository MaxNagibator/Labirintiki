namespace Labirint.Core.Tests;

[TestFixture]
public class TileTests : LabyrinthTestsBase
{
    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            Direction[] directions = [Direction.None, Direction.Left, Direction.Top, Direction.Right, Direction.Bottom];

            IEnumerable<(Direction direction, int count)> combinedDirections = directions.Where(direction => direction != Direction.None).GetCombinedDirections();

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

    [TestCaseSource(nameof(TestCases))]
    public void CanAddWallTest(Direction existingWalls, Direction directionToAdd, bool expectedResult)
    {
        Tile tile = new(Labyrinth)
        {
            Walls = existingWalls
        };

        bool result = tile.CanAddWall(directionToAdd);

        Assert.That(result, Is.EqualTo(expectedResult));
    }
}
