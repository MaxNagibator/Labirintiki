namespace Labirint.Core.Tests;

public abstract class LabyrinthTestsBase
{
    protected const int DefaultWidth = 16;
    protected const int DefaultHeight = 16;

    protected IRandom Random { get; private set; }
    protected Labyrinth Labyrinth { get; private set; }
    protected Inventory Inventory { get; private set; }

    [SetUp]
    public void SetUp()
    {
        Random = new TestRandom();
        Labyrinth = new Labyrinth(Random);
        Inventory = new Inventory();
        Labyrinth.Init(DefaultWidth, DefaultHeight, 40, Inventory.AllItems);
    }

    [TearDown]
    public void TearDown()
    {
        Inventory.Clear();
    }
}
