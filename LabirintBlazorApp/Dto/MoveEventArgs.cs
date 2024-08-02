using LabirintBlazorApp.Common;

namespace LabirintBlazorApp.Dto;

public class MoveEventArgs
{
    public int DeltaX => Direction.DeltaX;
    public int DeltaY => Direction.DeltaY;
    public required Key KeyCode { get; init; }
    public required (int DeltaX, int DeltaY) Direction { get; init; }
}
