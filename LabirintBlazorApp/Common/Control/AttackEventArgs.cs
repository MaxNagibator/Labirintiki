using Labirint.Core.Common;

namespace LabirintBlazorApp.Common.Control;

public class AttackEventArgs
{
    public required Key KeyCode { get; init; }
    public required Item? Item { get; init; }
    public Direction? Direction { get; init; }
}
