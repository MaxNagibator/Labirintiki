namespace Labirint.Core.Items;

public abstract class Item
{
    public string Name { get; init; }
    public string DisplayName { get; init; }

    /// <summary>
    ///     Количество в инвентаре
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    ///     Максимальное количество в инвентаре
    /// </summary>
    public int MaxCount { get; init; }

    /// <summary>
    ///     Количество лежащие в лабиринте
    /// </summary>
    public int InMazeCount { get; set; }

    public abstract int CalculateCountInMaze(int width, int height, int density);

    public virtual bool TryPeekUp()
    {
        if (Count >= MaxCount)
        {
            return false;
        }

        Count++;
        InMazeCount--;
        return true;
    }

    public virtual bool TryUse()
    {
        if (Count <= 0)
        {
            return false;
        }

        Count--;
        return true;
    }
}
