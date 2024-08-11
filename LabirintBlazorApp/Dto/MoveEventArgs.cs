using LabirintBlazorApp.Common;

namespace LabirintBlazorApp.Dto;

public class MoveEventArgs
{
    public required Key KeyCode { get; init; }
    public required Direction Direction { get; init; }
}
