using Labirint.Core.Items;
using Labirint.Core.TileFeatures;

namespace Labirint.Core.Tests.Items;

[TestFixture]
public class TileFeaturePickUpTests : LabyrinthTestsBase
{
    /// <summary>
    /// Тестирует, что Tile.TryPickUp успешно подбирает WorldItem, когда инвентарь может его принять.
    /// Проверяет, что метод возвращает true, отдаёт снятую особенность, очищает список особенностей клетки и увеличивает стек предмета в инвентаре бегуна на PickUpCount.
    /// </summary>
    [Test]
    public void WorldItemIsRemovedFromTileAfterSuccessfulPickUpTest()
    {
        var sand = Labyrinth.Runner.Inventory.AllItems.OfType<Sand>().Single();
        Tile tile = new(Labyrinth);
        WorldItem worldItem = new(sand, sand.Image, Alignment.Center, 1)
        {
            PickUpCount = 3,
        };
        tile.AddFeature(worldItem);

        var pickedUp = tile.TryPickUp(out var feature);

        Assert.Multiple(() =>
        {
            Assert.That(pickedUp, Is.True);
            Assert.That(feature, Is.SameAs(worldItem));
            Assert.That(tile.Features, Is.Empty);
            Assert.That(Labyrinth.Runner.Inventory.Stacks.Single(stack => stack.Item == sand).Count, Is.EqualTo(3));
        });
    }

    /// <summary>
    /// Тестирует, что Tile.TryPickUp не подбирает WorldItem, когда соответствующий стек инвентаря заполнен до MaxCount.
    /// Проверяет, что метод возвращает false, не отдаёт особенность, а сама особенность остаётся в списке особенностей клетки.
    /// </summary>
    [Test]
    public void WorldItemStaysOnTileWhenInventoryIsFullTest()
    {
        var hammer = Labyrinth.Runner.Inventory.AllItems.OfType<Hammer>().Single();
        var stack = Labyrinth.Runner.Inventory.Stacks.Single(itemStack => itemStack.Item == hammer);

        while (stack.TryAdd(1))
        {
        }

        Tile tile = new(Labyrinth);
        WorldItem worldItem = new(hammer, hammer.Image, Alignment.Center, 1);
        tile.AddFeature(worldItem);

        var pickedUp = tile.TryPickUp(out var feature);

        Assert.Multiple(() =>
        {
            Assert.That(stack.Count, Is.EqualTo(stack.MaxCount));
            Assert.That(pickedUp, Is.False);
            Assert.That(feature, Is.Null);
            Assert.That(tile.Features, Has.Count.EqualTo(1));
        });
    }

    /// <summary>
    /// Тестирует, что Tile.TryPickUp не подбирает декоративную WoolYarnFeature.
    /// Проверяет, что метод возвращает false, не отдаёт особенность, а сама WoolYarnFeature остаётся в списке особенностей клетки той же ссылкой.
    /// </summary>
    [Test]
    public void WoolYarnFeatureStaysOnTileAfterPickUpAttemptTest()
    {
        Tile tile = new(Labyrinth);
        WoolYarnFeature feature = new(Direction.Right);
        tile.AddFeature(feature);

        var pickedUp = tile.TryPickUp(out var pickedFeature);

        Assert.Multiple(() =>
        {
            Assert.That(pickedUp, Is.False);
            Assert.That(pickedFeature, Is.Null);
            Assert.That(tile.Features, Has.Count.EqualTo(1));
            Assert.That(tile.Features![0], Is.SameAs(feature));
        });
    }
}
