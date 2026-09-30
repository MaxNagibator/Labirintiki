using Labirint.Core.Items;

namespace Labirint.Core.Tests.Items;

[TestFixture]
public class ItemStatsTests
{
    /// <summary>
    /// Тестирует, что предмет сам объявляет свой вид и свои характеристики для витрины, без разбора типа на стороне потребителя.
    /// Проверяет, что сокровища зовутся сокровищами и несут характеристику ценности со своей стоимостью за штуку, а снаряжение зовётся снаряжением и ценности не имеет.
    /// </summary>
    /// <param name="itemType">Тип предмета.</param>
    /// <param name="expectedKind">Ожидаемый вид предмета.</param>
    /// <param name="expectedCost">Ожидаемая стоимость за штуку или null, если предмет её не объявляет.</param>
    [TestCase(typeof(Sand), "Сокровище", 100)]
    [TestCase(typeof(Oil), "Сокровище", 100_000)]
    [TestCase(typeof(Hammer), "Снаряжение", null)]
    [TestCase(typeof(WalkThroughWallsBottle), "Снаряжение", null)]
    public void ItemDescribesOwnKindAndStatsTest(Type itemType, string expectedKind, int? expectedCost)
    {
        var item = ItemCatalog.Items.Single(item => item.GetType() == itemType);

        var cost = item.Stats.SingleOrDefault(stat => stat.Label == "Ценность")?.Value;

        Assert.Multiple(() =>
        {
            Assert.That(item.Kind, Is.EqualTo(expectedKind));
            Assert.That(cost, Is.EqualTo(expectedCost?.ToString()));
        });
    }
}
