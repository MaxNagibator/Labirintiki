namespace Labirint.Web.Components.Ui;

public static class IconCatalog
{
    public const string Menu = "menu";
    public const string Close = "close";
    public const string Home = "home";
    public const string Maze = "maze";
    public const string Settings = "settings";
    public const string Gear = "gear";
    public const string Theme = "theme";
    public const string Share = "share";
    public const string Copy = "copy";
    public const string Coin = "coin";
    public const string ChevronDown = "chevron-down";
    public const string ChevronUp = "chevron-up";
    public const string ChevronRight = "chevron-right";
    public const string Info = "info";
    public const string Check = "check";
    public const string Warning = "warning";
    public const string Error = "error";
    public const string Refresh = "refresh";
    public const string Play = "play";
    public const string Pause = "pause";

    private static readonly Dictionary<string, string> Glyphs = new()
    {
        [Menu] = """<path d="M4 7h16M4 12h16M4 17h16"/>""",
        [Close] = """<path d="m6 6 12 12M18 6 6 18"/>""",
        [Home] = """<path d="M4 10.5 12 4l8 6.5V20a1 1 0 0 1-1 1h-4v-6H9v6H5a1 1 0 0 1-1-1z"/>""",
        [Maze] = """<path d="M3 3h18v18H3z"/><path d="M7 3v7h10M3 14h7v7M14 21v-7h7"/>""",
        [Settings] = """<path d="M4 7h9M17 7h3M4 12h3M11 12h9M4 17h7M15 17h5"/><circle cx="15" cy="7" r="2"/><circle cx="9" cy="12" r="2"/><circle cx="13" cy="17" r="2"/>""",
        [Gear] = """<circle cx="12" cy="12" r="7"/><circle cx="12" cy="12" r="2.6"/><path d="M12 3v2M12 19v2M3 12h2M19 12h2m-2.64-6.36-1.41 1.41M7.05 16.95l-1.41 1.41m12.9 0-1.41-1.41M7.05 7.05 5.64 5.64"/>""",
        [Theme] = """<circle cx="12" cy="12" r="8"/><path d="M12 4a8 8 0 0 1 0 16z" fill="currentColor" stroke="none"/>""",
        [Share] = """<circle cx="18" cy="5" r="2.5"/><circle cx="6" cy="12" r="2.5"/><circle cx="18" cy="19" r="2.5"/><path d="m8.3 10.8 7.4-4.4M8.3 13.2l7.4 4.4"/>""",
        [Copy] = """<rect x="9" y="9" width="11" height="11" rx="1.5"/><path d="M6 15H5a1 1 0 0 1-1-1V5a1 1 0 0 1 1-1h9a1 1 0 0 1 1 1v1"/>""",
        [Coin] = """<circle cx="12" cy="12" r="8"/><path d="m12 8 2.2 4-2.2 4-2.2-4z"/>""",
        [ChevronDown] = """<path d="m6 9 6 6 6-6"/>""",
        [ChevronUp] = """<path d="m6 15 6-6 6 6"/>""",
        [ChevronRight] = """<path d="m9 6 6 6-6 6"/>""",
        [Info] = """<circle cx="12" cy="12" r="8"/><path d="M12 11v5M12 8h.01"/>""",
        [Check] = """<path d="m5 13 4.5 4.5L19 7"/>""",
        [Warning] = """<path d="M12 4 3 19h18z"/><path d="M12 10v4M12 17h.01"/>""",
        [Error] = """<circle cx="12" cy="12" r="8"/><path d="M12 8v5M12 16h.01"/>""",
        [Refresh] = """<path d="M20 12a8 8 0 1 1-2.6-5.9"/><path d="M20 4v4h-4"/>""",
        [Play] = """<path d="M8 5.5v13l11-6.5z"/>""",
        [Pause] = """<path d="M9 5v14M15 5v14"/>""",
    };

    public static string Get(string name)
    {
        return Glyphs.TryGetValue(name, out var glyph) ? glyph : string.Empty;
    }
}
