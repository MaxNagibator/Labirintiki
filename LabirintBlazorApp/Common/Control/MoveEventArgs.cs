using Labirint.Core.Common;

namespace LabirintBlazorApp.Common.Control;

public class MoveEventArgs
{
    public required Key KeyCode { get; init; }
    public required Direction Direction { get; init; }
}
