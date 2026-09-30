using Labirint.Core.TileFeatures;

namespace Labirint.Core.Items.Base;

public abstract class Item
{
    public abstract string Name { get; }
    public abstract string DisplayName { get; }
    public abstract string Description { get; }

    public abstract int DefaultCount { get; }
    public abstract int MaxCount { get; }

    /// <summary>
    /// Используется сразу после подбора.
    /// </summary>
    public virtual bool IsUsedOnPickup => false;

    /// <summary>
    /// Вид предмета для витрины.
    /// </summary>
    public virtual string Kind => "Снаряжение";

    /// <summary>
    /// Характеристики предмета для витрины.
    /// </summary>
    public virtual IReadOnlyList<ItemStat> Stats => [];

    /// <summary>
    /// Корень ресурсов предмета: у предметов из отдельной сборки это её статические файлы.
    /// </summary>
    public virtual string ResourceRoot => "images/items";

    public string Icon => $"{ResourceRoot}/{Name}-icon.webp";
    public string Image => $"{ResourceRoot}/{Name}.webp";

    public virtual ControlSettings? ControlSettings => null;
    public virtual SoundSettings? SoundSettings => null;

    /// <summary>
    /// Применить предмет. Отсутствие направления и <see cref="Direction.None" /> равнозначны: предмет получает null.
    /// </summary>
    /// <param name="position">Позиция, из которой предмет применяется.</param>
    /// <param name="direction">Направление применения или null, если направления нет.</param>
    /// <param name="labyrinth">Лабиринт, в котором предмет применяется.</param>
    public void Use(Position position, Direction? direction, Labyrinth labyrinth)
    {
        AfterUse(position, direction == Direction.None ? null : direction, labyrinth);
    }

    /// <summary>
    /// Подобрать предмет: по умолчанию он отправляется в инвентарь.
    /// </summary>
    /// <param name="count">Количество подобранных штук.</param>
    /// <param name="context">Возможности подобравшего.</param>
    /// <returns>True, если предмет принят и должен исчезнуть с поля; иначе false.</returns>
    public virtual bool TryPickUp(int count, IPickupContext context)
    {
        return context.TryStore(this, count);
    }

    public abstract int CalculateCountInMaze(int width, int height, int density);

    public IEnumerable<WorldItem> GetItemsForPlace(WorldItemParameters parameters)
    {
        var requiredCount = CalculateCountInMaze(parameters.Width, parameters.Height, parameters.Density);

        for (var i = 0; i < requiredCount; i++)
        {
            yield return GetWorldItem(parameters);
        }
    }

    protected virtual WorldItem GetWorldItem(WorldItemParameters parameters)
    {
        return new(this, Image, Alignment.Center, 0.9)
        {
            AfterPlace = AfterPlace,
        };
    }

    protected virtual void AfterUse(Position position, Direction? direction, Labyrinth labyrinth)
    {
    }

    protected virtual void AfterPlace(Position position, Labyrinth labyrinth)
    {
    }
}
