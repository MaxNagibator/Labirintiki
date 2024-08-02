using LabirintBlazorApp.Common;

namespace LabirintBlazorApp.Dto;

public class AttackEventArgs
{
    public required AttackType Type { get; set; }
    public required Key KeyCode { get; set; }
}
