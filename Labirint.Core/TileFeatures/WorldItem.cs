namespace Labirint.Core.TileFeatures;

public class WorldItem(Item item, string imageSource, Alignment alignment, double scale)
    : TileFeature
{
    /// <summary>
    /// Предмет, лежащий в клетке.
    /// </summary>
    public Item Item => item;

    public Action<Position, Labyrinth>? AfterPlace { get; init; }
    public int? PickUpCount { get; init; }

    public override bool RemoveAfterSuccessPickUp => true;
    public override string PickUpSound => item.SoundSettings?.PickUpSound ?? string.Empty;
    public override DrawingSettings? DrawingSettings { get; } = new(imageSource, alignment, scale, 1);

    /// <summary>
    /// Попытаться подобрать предмет.
    /// </summary>
    /// <param name="labyrinth">Лабиринт.</param>
    /// <returns>Если предмет пропал с лабиринта, то true.</returns>
    public override bool TryPickUp(Labyrinth labyrinth)
    {
        return item.TryPickUp(PickUpCount ?? 1, new PickupContext(labyrinth));
    }
}
