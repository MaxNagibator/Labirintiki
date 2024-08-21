using Labirint.Core.Common;

namespace Labirint.Core.Tests.Helpers;

internal static class DirectionExtensions
{
    internal static Direction[] GetAll()
    {
        return [Direction.Left, Direction.Top, Direction.Right, Direction.Bottom];
    }
}
