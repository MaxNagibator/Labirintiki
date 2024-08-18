namespace Labirint.Core.Common;

public record Key
{
    public static readonly Key ArrowUp = new(nameof(ArrowUp), "↑");
    public static readonly Key ArrowDown = new(nameof(ArrowDown), "↓");
    public static readonly Key ArrowLeft = new(nameof(ArrowLeft), "←");
    public static readonly Key ArrowRight = new(nameof(ArrowRight), "→");

    public static readonly Key KeyA = new(nameof(KeyA), "A");
    public static readonly Key KeyB = new(nameof(KeyB), "B");
    public static readonly Key KeyW = new(nameof(KeyW), "W");
    public static readonly Key KeyS = new(nameof(KeyS), "S");
    public static readonly Key KeyD = new(nameof(KeyD), "D");

    public static readonly Key Space = new(nameof(Space), "␣");
    public static readonly Key ControlLeft = new(nameof(ControlLeft), "Ctrl");

    public static readonly Key Undefined = new(nameof(Undefined), "?");

    private static readonly Key[] All =
    [
        ArrowUp, ArrowDown, ArrowLeft, ArrowRight,
        KeyA, KeyB, KeyW, KeyS, KeyD, Space, ControlLeft
    ];

    private Key(string keyCode, string displaySymbol)
    {
        KeyCode = keyCode;
        DisplaySymbol = displaySymbol;
    }

    public string KeyCode { get; }
    public string DisplaySymbol { get; }

    public static Key Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new Key(Undefined.KeyCode, Undefined.DisplaySymbol);
        }

        string keyCode = input.Trim();

        return All.Any(key => key.KeyCode.Equals(keyCode, StringComparison.CurrentCulture))
            ? new Key(keyCode, All.First(key => key.KeyCode.Equals(keyCode, StringComparison.CurrentCulture)).DisplaySymbol)
            : new Key(Undefined.KeyCode, Undefined.DisplaySymbol);
    }

    public static implicit operator string(Key key)
    {
        return key.KeyCode;
    }
}
