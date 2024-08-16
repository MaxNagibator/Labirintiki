using Labirint.Core.Interfaces;
using Labirint.Core.Items;
using Labirint.Core.Stacks;
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
        _inventory = new Inventory();
        _labyrinth.Init(Width, Height, Density, _inventory.Items.Select(x => x.Item));
    }

    [TearDown]
    public void TearDown()
    {
        _inventory.Clear();
    }

    private const int Width = 16;
    private const int Height = 16;
    private const int Density = 40;

    private IRandom _randomMock;
    private Labyrinth _labyrinth;
    private Inventory _inventory;

    /// <summary>
    ///     Была ошибка, что выдавались только песочки.
    /// </summary>
    [Test]
    public void PlacedCorrectCountOfItemsTest()
    {
        foreach (ItemStack itemStack in _inventory.Items)
        {
            (Item item, int expectedCount) = (itemStack.Item, itemStack.Item.CalculateCountInMaze(Width, Height, Density));

            // TODO исправить костыль с определение по пути
            int count = _labyrinth.Enumerate()
                .Where(tile => tile.WorldItem != null)
                .Count(tile => tile.WorldItem!.ImageSource.Contains(item.Name));

            Console.WriteLine($"{item.Name}: {count}/{expectedCount}");
            Assert.That(count, Is.EqualTo(expectedCount));
        }
    }
}
