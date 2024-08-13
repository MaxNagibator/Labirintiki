using Labirint.Core.Interfaces;
using Labirint.Core.Items;
using Labirint.Core.Tests.Helpers;

namespace Labirint.Core.Tests;

[TestFixture]
public class LabyrinthTests
{
    [SetUp]
    public void SetUp()
    {
        _randomMock = new TestRandom();
        _labyrinth = new Labyrinth(_randomMock);
        _labyrinth.Init(Width, Height, Density);
    }

    private const int Width = 16;
    private const int Height = 16;
    private const int Density = 40;

    private IRandom _randomMock;
    private Labyrinth _labyrinth;

    private static readonly Item[] TestItems =
    [
        new Sand(),
        new Hammer(),
        new Bomb()
    ];

    /// <summary>
    ///     Была ошибка, что выдавались только песочки.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(TestItems))]
    public void PlacedCorrectCountOfItemsTest(Item expected)
    {
        (Item item, int expectedCount) = (expected, expected.CalculateCountInMaze(Width, Height, Density));

        int count = _labyrinth.Enumerate()
            .Where(x => x.ItemType != null)
            .Count(x => x.ItemType!.Name == item.Name);

        Assert.That(count, Is.EqualTo(expectedCount));
    }
}
