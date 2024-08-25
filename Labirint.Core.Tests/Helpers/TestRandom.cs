namespace Labirint.Core.Tests.Helpers;

internal class TestRandom : IRandom
{
    public Random Generator { get; } = new(1);
}
