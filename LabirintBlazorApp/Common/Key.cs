namespace LabirintBlazorApp.Common;

public class Key : ValueObject
{
    public static readonly Key ArrowUp = new(nameof(ArrowUp));
    public static readonly Key ArrowDown = new(nameof(ArrowDown));
    public static readonly Key ArrowLeft = new(nameof(ArrowLeft));
    public static readonly Key ArrowRight = new(nameof(ArrowRight));

    public static readonly Key KeyA = new(nameof(KeyA));
    public static readonly Key KeyB = new(nameof(KeyB));
    public static readonly Key KeyW = new(nameof(KeyW));
    public static readonly Key KeyS = new(nameof(KeyS));
    public static readonly Key KeyD = new(nameof(KeyD));

    public static readonly Key Space = new(nameof(Space));
    public static readonly Key ControlLeft = new(nameof(ControlLeft));

    public static readonly Key Undefined = new(nameof(Undefined));

    private static readonly Key[] All =
    [
        ArrowUp, ArrowDown, ArrowLeft, ArrowRight,
        KeyA, KeyB, KeyW, KeyS, KeyD, Space, ControlLeft
    ];

    private Key(string keyCode)
    {
        KeyCode = keyCode;
    }

    public string KeyCode { get; }

    public static Key Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new Key(Undefined.KeyCode);
        }

        string keyCode = input.Trim();

        return All.Any(key => key.KeyCode.Equals(keyCode, StringComparison.CurrentCulture))
            ? new Key(keyCode)
            : new Key(Undefined.KeyCode);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return KeyCode;
    }

    public static implicit operator string(Key key)
    {
        return key.KeyCode;
    }
}
