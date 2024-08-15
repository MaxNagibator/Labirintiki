namespace Labirint.Core.Items;

public record WorldItem
{
    public required string ImageSource { get; init; }
    public required string PickUpSound { get; init; }
    public required Alignment Alignment { get; init; }
    public required double Scale { get; init; }

    public required Func<bool> PickUp { get; init; }
}
