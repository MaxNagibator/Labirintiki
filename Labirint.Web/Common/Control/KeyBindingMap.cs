using Labirint.Web.Common.Control.Schemes;

namespace Labirint.Web.Common.Control;

public sealed class KeyBindingMap
{
    private const string DigitPrefix = "Digit";

    private Dictionary<string, Direction> _directions = new();
    private Dictionary<string, Item> _items = new();

    public static int? FindDigit(string code)
    {
        return code.StartsWith(DigitPrefix) && char.IsDigit(code[^1])
            ? code[^1] - '0'
            : null;
    }

    public void Rebuild(IControlScheme scheme, Inventory? inventory)
    {
        _directions = new()
        {
            [scheme.MoveLeft] = Direction.Left,
            [scheme.MoveUp] = Direction.Top,
            [scheme.MoveRight] = Direction.Right,
            [scheme.MoveDown] = Direction.Bottom,
        };

        RebuildItems(scheme, inventory);
    }

    public void RebuildItems(IControlScheme scheme, Inventory? inventory)
    {
        if (inventory == null)
        {
            return;
        }

        _items = inventory.AllItems
            .Where(item => item.ControlSettings != null)
            .Select(item => (scheme.GetActivateKey(item.ControlSettings!).KeyCode, item))
            .ToDictionary();
    }

    public Direction? FindDirection(string code)
    {
        return _directions.TryGetValue(code, out var direction) ? direction : null;
    }

    public Item? FindItem(string code)
    {
        return _items.GetValueOrDefault(code);
    }
}
