namespace LabirintBlazorApp.Common.Control;

public class AttackEventArgs
{
    public required Item? Item { get; init; }
    public Direction? Direction { get; init; }
}
