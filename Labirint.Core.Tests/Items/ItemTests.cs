using Labirint.Core.Items;

namespace Labirint.Core.Tests.Items;

[TestFixture]
public class ItemTests
{
    private static IEnumerable<TestCaseData> CountInMazeCases
    {
        get
        {
            (int Width, int Height, int Density, (Type Type, int Count)[] Expected)[] mazes =
            [
                (16, 16, 40, [(typeof(Sand), 16), (typeof(Hammer), 3), (typeof(Bomb), 1), (typeof(Oil), 0)]),
                (32, 32, 20, [(typeof(Sand), 32), (typeof(Hammer), 3), (typeof(Bomb), 1), (typeof(Oil), 1)]),
            ];

            foreach (var (width, height, density, expected) in mazes)
            {
                foreach (var (type, count) in expected)
                {
                    yield return new TestCaseData(type, width, height, density, count);
                }
            }
        }
    }

    /// <summary>
    /// Тестирует, что метод расчета количества предметов правильно рассчитывает количество предметов в лабиринте.
    /// Проверяет, что расчетное количество равно ожидаемому.
    /// </summary>
    /// <param name="itemType">Тип предмета</param>
    /// <param name="width">Ширина лабиринта</param>
    /// <param name="height">Высота лабиринта</param>
    /// <param name="density">Плотность стен в лабиринте</param>
    /// <param name="expectedCount">Ожидаемое количество предметов</param>
    [TestCaseSource(nameof(CountInMazeCases))]
    public void ItemsCorrectCalculateCountInMazeTest(Type itemType, int width, int height, int density, int expectedCount)
    {
        var item = (Item)Activator.CreateInstance(itemType)!;

        var count = item.CalculateCountInMaze(width, height, density);

        Assert.That(count, Is.EqualTo(expectedCount));
    }
}
