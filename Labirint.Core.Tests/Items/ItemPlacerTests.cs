namespace Labirint.Core.Tests.Items;

[TestFixture]
public class ItemPlacerTests
{
    /// <summary>
    /// Тестирует, что ItemPlacer размещает предметы в лабиринте без повторов позиций.
    /// Проверяет, что все размещения уникальны, лежат в границах поля и разбиты по предметам согласно заданному количеству.
    /// </summary>
    [Test]
    public void PlacesEachItemAtUniquePositionWithinBoundsTest()
    {
        List<(int X, int Y, Item Item)> placements = [];
        SeedRandom random = new(1);
        ItemPlacer placer = new(random, (x, y, item) => placements.Add((x, y, item.Item)));
        List<TestItem> items = [new(3), new(2)];

        placer.PlaceItems(5, 5, 40, items);

        Assert.Multiple(() =>
        {
            Assert.That(placements, Has.Count.EqualTo(5));
            Assert.That(placements.Select(p => (p.X, p.Y)).Distinct().Count(), Is.EqualTo(5));
            Assert.That(placements.All(p => p.X is >= 0 and < 5 && p.Y is >= 0 and < 5), Is.True);
            Assert.That(placements.Count(p => p.Item == items[0]), Is.EqualTo(3));
            Assert.That(placements.Count(p => p.Item == items[1]), Is.EqualTo(2));
        });
    }

    /// <summary>
    /// Тестирует, что ItemPlacer корректно размещает предметы в неквадратном лабиринте.
    /// Проверяет, что все позиции размещения лежат в границах ширины и высоты поля.
    /// </summary>
    /// <param name="width">Ширина лабиринта</param>
    /// <param name="height">Высота лабиринта</param>
    [TestCase(4, 3)]
    [TestCase(3, 5)]
    public void PlacesItemsWithinBoundsOfNonSquareMazeTest(int width, int height)
    {
        List<(int X, int Y)> placements = [];
        SeedRandom random = new(1);
        ItemPlacer placer = new(random, (x, y, _) => placements.Add((x, y)));
        List<TestItem> items = [new(4)];

        placer.PlaceItems(width, height, 40, items);

        Assert.Multiple(() =>
        {
            Assert.That(placements, Is.Not.Empty);
            Assert.That(placements.All(position => position.X >= 0 && position.X < width), Is.True);
            Assert.That(placements.All(position => position.Y >= 0 && position.Y < height), Is.True);
        });
    }

    private static IEnumerable<TestCaseData> SqueezeCases
    {
        get
        {
            yield return new TestCaseData(4, 4, new[] { 10, 6, 4 }, new[] { 8, 4, 3 });
            yield return new TestCaseData(3, 3, new[] { 5, 5, 5 }, new[] { 3, 3, 2 });
        }
    }

    /// <summary>
    /// Тестирует, что ItemPlacer ужимает количества предметов, когда их суммарный спрос превышает число свободных клеток.
    /// Проверяет, что итоговое число размещений равно числу свободных клеток и что оно распределено между предметами пропорционально ожидаемым значениям.
    /// </summary>
    /// <param name="width">Ширина лабиринта</param>
    /// <param name="height">Высота лабиринта</param>
    /// <param name="counts">Запрошенные количества каждого предмета</param>
    /// <param name="expectedCounts">Ожидаемые количества после ужимания</param>
    [TestCaseSource(nameof(SqueezeCases))]
    public void SqueezesProportionallyWhenDemandExceedsFreeCellsTest(int width, int height, int[] counts, int[] expectedCounts)
    {
        List<(int X, int Y, Item Item)> placements = [];
        SeedRandom random = new(1);
        ItemPlacer placer = new(random, (x, y, item) => placements.Add((x, y, item.Item)));
        var items = counts.Select(count => new TestItem(count)).ToList();
        var length = width * height - 1;

        placer.PlaceItems(width, height, 40, items);

        Assert.Multiple(() =>
        {
            Assert.That(placements, Has.Count.EqualTo(length));
            Assert.That(placements.Select(p => (p.X, p.Y)).Distinct().Count(), Is.EqualTo(length));

            for (var i = 0; i < items.Count; i++)
            {
                Assert.That(placements.Count(p => p.Item == items[i]), Is.EqualTo(expectedCounts[i]));
            }
        });
    }
}
