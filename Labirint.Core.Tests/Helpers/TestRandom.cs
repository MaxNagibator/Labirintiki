namespace Labirint.Core.Tests.Helpers;

internal class TestRandom : IRandom
{
    public Random Random => new(1);
}
