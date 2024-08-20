using Labirint.Core.Common;
using Labirint.Core.Items;
using Labirint.Core.Stacks;

namespace Labirint.Core.Tests.Helpers;

internal class TestItem : Item
{
    private static int _id;

    public TestItem(int count)
    {
        Count = count;
        Name = "test " + _id++;
        DisplayName = "Test " + count;

        Stack = new LimitedItemStack(this, 2, 2);

        ControlSettings = new ControlSettings(Key.KeyB, Key.ControlLeft);
        SoundSettings = new SoundSettings("bomb", "score");
    }

    public int Count { get; }

    public override int CalculateCountInMaze(int width, int height, int density)
    {
        return Count;
    }
}
