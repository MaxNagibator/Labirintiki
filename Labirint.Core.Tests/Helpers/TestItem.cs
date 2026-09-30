namespace Labirint.Core.Tests.Helpers;

internal class TestItem(int count) : Item
{
    private static int _id;

    public override string Name { get; } = "test " + _id++;
    public override string DisplayName { get; } = "Test " + count;
    public override string Description => string.Empty;

    public override int DefaultCount => 0;
    public override int MaxCount => 0;

    public int Count { get; } = count;

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return Count;
    }
}
