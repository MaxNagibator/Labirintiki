using Labirint.Core.Abilities;
using Labirint.Core.Items;
using Labirint.Core.TileFeatures;

namespace Labirint.Core.Tests.Items;

[TestFixture]
public class ItemPickupTests : LabyrinthTestsBase
{
    /// <summary>
    /// Тестирует, что очки начисляет сам подобранный предмет, а не разбор его типа в инвентаре.
    /// Проверяет, что счёт бегуна растёт на стоимость предмета, умноженную на количество, событие ScoreIncreased поднимается один раз с той же суммой, а предмет без стоимости очков не приносит и события не поднимает.
    /// </summary>
    /// <param name="itemType">Тип подбираемого предмета.</param>
    /// <param name="count">Количество подбираемых штук.</param>
    /// <param name="expectedScore">Ожидаемая сумма начисленных очков.</param>
    [TestCase(typeof(Sand), 3, 300)]
    [TestCase(typeof(Oil), 2, 200_000)]
    [TestCase(typeof(WoolYarn), 1, 0)]
    public void ScoreIsGrantedByItemItselfTest(Type itemType, int count, int expectedScore)
    {
        var item = Labyrinth.Runner.Inventory.AllItems.Single(item => item.GetType() == itemType);
        List<int> amounts = [];
        Labyrinth.Runner.ScoreIncreased += (_, amount) => amounts.Add(amount);

        var pickedUp = PickUp(item, count);

        Assert.Multiple(() =>
        {
            Assert.That(pickedUp, Is.True);
            Assert.That(Labyrinth.Runner.Score, Is.EqualTo(expectedScore));
            Assert.That(amounts, Has.Count.EqualTo(expectedScore == 0 ? 0 : 1));
            Assert.That(amounts.Sum(), Is.EqualTo(expectedScore));
        });
    }

    /// <summary>
    /// Тестирует, что зелье прохождения сквозь стены применяется прямо при подборе, минуя инвентарь.
    /// Проверяет, что подбор считается успешным, бегун получает способность прохождения сквозь стены, а стек зелья в инвентаре остаётся пустым.
    /// </summary>
    [Test]
    public void BottleIsUsedOnPickupInsteadOfStoringTest()
    {
        var bottle = Labyrinth.Runner.Inventory.AllItems.OfType<WalkThroughWallsBottle>().Single();

        var pickedUp = PickUp(bottle, 1);

        Assert.Multiple(() =>
        {
            Assert.That(pickedUp, Is.True);
            Assert.That(Labyrinth.Runner.Abilities.Select(ability => ability.Properties), Has.Exactly(1).InstanceOf<WalkThroughWallsAbility>());
            Assert.That(Labyrinth.Runner.Inventory.Stacks.Single(stack => stack.Item == bottle).Count, Is.Zero);
        });
    }

    /// <summary>
    /// Тестирует, что начисление очков за огромную россыпь сокровищ не переполняет счёт бегуна.
    /// Проверяет, что счёт упирается в int.MaxValue и остаётся положительным.
    /// </summary>
    [Test]
    public void ScoreIsSaturatedInsteadOfOverflowingTest()
    {
        var sand = Labyrinth.Runner.Inventory.AllItems.OfType<Sand>().Single();

        var pickedUp = PickUp(sand, int.MaxValue);

        Assert.Multiple(() =>
        {
            Assert.That(pickedUp, Is.True);
            Assert.That(Labyrinth.Runner.Score, Is.EqualTo(int.MaxValue));
        });
    }

    /// <summary>
    /// Тестирует, что подбор отвергает бессмысленное количество штук, а не уводит стек и счёт в минус.
    /// Проверяет, что подбор не удался, стек предмета остался пустым, а счёт бегуна не изменился.
    /// </summary>
    /// <param name="count">Количество подбираемых штук.</param>
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public void NonPositivePickUpCountIsRejectedTest(int count)
    {
        var sand = Labyrinth.Runner.Inventory.AllItems.OfType<Sand>().Single();

        var pickedUp = PickUp(sand, count);

        Assert.Multiple(() =>
        {
            Assert.That(pickedUp, Is.False);
            Assert.That(Labyrinth.Runner.Inventory.Stacks.Single(stack => stack.Item == sand).Count, Is.Zero);
            Assert.That(Labyrinth.Runner.Score, Is.Zero);
        });
    }

    private bool PickUp(Item item, int count)
    {
        Tile tile = new(Labyrinth);
        WorldItem worldItem = new(item, item.Image, Alignment.Center, 1)
        {
            PickUpCount = count,
        };

        tile.AddFeature(worldItem);

        return tile.TryPickUp(out _);
    }
}
