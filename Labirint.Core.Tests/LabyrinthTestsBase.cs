namespace Labirint.Core.Tests;

public abstract class LabyrinthTestsBase
{
    protected const int DefaultWidth = 16;
    protected const int DefaultHeight = 16;

    private const int DefaultSeed = 1;

    protected IRandom Random { get; private set; }
    protected Labyrinth Labyrinth { get; private set; }
    protected Inventory Inventory { get; private set; }

    [SetUp]
    public void SetUp()
    {
        Random = new SeedRandom(DefaultSeed);
        Labyrinth = new(Random);
        Inventory = new();
        Labyrinth.Init(DefaultWidth, DefaultHeight, 40, Inventory.AllItems);
    }

    [TearDown]
    public void TearDown()
    {
        Inventory.Clear();
    }
}
