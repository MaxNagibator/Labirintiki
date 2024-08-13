using LabirintBlazorApp.Constants;

namespace LabirintBlazorApp.Common.Control;

public class AttackEventArgs
{
    public required AttackType Type { get; init; }
    public required Key KeyCode { get; init; }
}
