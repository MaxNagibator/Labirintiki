using LabirintBlazorApp.Common;

namespace LabirintBlazorApp.Dto;

public class AttackEventArgs
{
    public required AttackType Type { get; init; }
    public required Key KeyCode { get; init; }
}
