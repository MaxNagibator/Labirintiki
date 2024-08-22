namespace Labirint.Core.Common;

public record Key
{
    public static readonly Key ArrowUp = new(nameof(ArrowUp), "↑");
    public static readonly Key ArrowDown = new(nameof(ArrowDown), "↓");
    public static readonly Key ArrowLeft = new(nameof(ArrowLeft), "←");
    public static readonly Key ArrowRight = new(nameof(ArrowRight), "→");

    public static readonly Key KeyA = new(nameof(KeyA), "A");
    public static readonly Key KeyB = new(nameof(KeyB), "B");
    public static readonly Key KeyC = new(nameof(KeyC), "C");
    public static readonly Key KeyD = new(nameof(KeyD), "D");
    public static readonly Key KeyE = new(nameof(KeyE), "E");
    public static readonly Key KeyF = new(nameof(KeyF), "F");
    public static readonly Key KeyG = new(nameof(KeyG), "G");
    public static readonly Key KeyH = new(nameof(KeyH), "H");
    public static readonly Key KeyI = new(nameof(KeyI), "I");
    public static readonly Key KeyJ = new(nameof(KeyJ), "J");
    public static readonly Key KeyK = new(nameof(KeyK), "K");
    public static readonly Key KeyL = new(nameof(KeyL), "L");
    public static readonly Key KeyM = new(nameof(KeyM), "M");
    public static readonly Key KeyN = new(nameof(KeyN), "N");
    public static readonly Key KeyO = new(nameof(KeyO), "O");
    public static readonly Key KeyP = new(nameof(KeyP), "P");
    public static readonly Key KeyQ = new(nameof(KeyQ), "Q");
    public static readonly Key KeyR = new(nameof(KeyR), "R");
    public static readonly Key KeyS = new(nameof(KeyS), "S");
    public static readonly Key KeyT = new(nameof(KeyT), "T");
    public static readonly Key KeyU = new(nameof(KeyU), "U");
    public static readonly Key KeyV = new(nameof(KeyV), "V");
    public static readonly Key KeyW = new(nameof(KeyW), "W");
    public static readonly Key KeyX = new(nameof(KeyX), "X");
    public static readonly Key KeyY = new(nameof(KeyY), "Y");
    public static readonly Key KeyZ = new(nameof(KeyZ), "Z");

    public static readonly Key Space = new(nameof(Space), "␣");
    public static readonly Key ControlLeft = new(nameof(ControlLeft), "LCtrl");
    public static readonly Key ControlRight = new(nameof(ControlRight), "RCtrl");
    public static readonly Key ShiftLeft = new(nameof(ShiftLeft), "LShift");
    public static readonly Key ShiftRight = new(nameof(ShiftRight), "RShift");
    public static readonly Key AltLeft = new(nameof(AltLeft), "LAlt");
    public static readonly Key AltRight = new(nameof(AltRight), "RAlt");
    public static readonly Key Enter = new(nameof(Enter), "Enter");
    public static readonly Key Backspace = new(nameof(Backspace), "Backspace");
    public static readonly Key Escape = new(nameof(Escape), "Esc");
    public static readonly Key Tab = new(nameof(Tab), "Tab");
    public static readonly Key CapsLock = new(nameof(CapsLock), "CapsLock");
    public static readonly Key Delete = new(nameof(Delete), "Del");
    public static readonly Key Insert = new(nameof(Insert), "Ins");
    public static readonly Key Home = new(nameof(Home), "Home");
    public static readonly Key End = new(nameof(End), "End");
    public static readonly Key PageUp = new(nameof(PageUp), "PgUp");
    public static readonly Key PageDown = new(nameof(PageDown), "PgDn");
    public static readonly Key PrintScreen = new(nameof(PrintScreen), "PrtSc");
    public static readonly Key ScrollLock = new(nameof(ScrollLock), "ScrLk");
    public static readonly Key PauseBreak = new(nameof(PauseBreak), "Pause");
    public static readonly Key NumLock = new(nameof(NumLock), "NumLk");

    public static readonly Key Undefined = new(nameof(Undefined), "?");

    private static readonly Key[] All =
    [
        ArrowUp, ArrowDown, ArrowLeft, ArrowRight,
        KeyA, KeyB, KeyC, KeyD, KeyE, KeyF, KeyG, KeyH, KeyI, KeyJ, KeyK, KeyL, KeyM, KeyN, KeyO, KeyP, KeyQ, KeyR, KeyS, KeyT, KeyU, KeyV, KeyW, KeyX, KeyY, KeyZ,
        Space, ControlLeft, ControlRight, ShiftLeft, ShiftRight, AltLeft, AltRight, Enter, Backspace, Escape, Tab,
        CapsLock, Delete, Insert, Home, End, PageUp, PageDown, PrintScreen, ScrollLock, PauseBreak, NumLock
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
