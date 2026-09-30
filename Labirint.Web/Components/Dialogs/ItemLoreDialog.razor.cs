using Labirint.Core.Items.Common;
using Labirint.Web.Common.Control.Schemes;
using Labirint.Web.Services.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components.Dialogs;

public partial class ItemLoreDialog
{
    private IReadOnlyList<LoreSection> _sections = [];

    [Parameter]
    [EditorRequired]
    public required Item Item { get; set; }

    [Parameter]
    public int Count { get; set; }

    [Parameter]
    public bool IsInfinite { get; set; }

    [CascadingParameter]
    private DialogInstance Instance { get; set; } = null!;

    [Inject]
    private ControlSchemeService SchemeService { get; set; } = null!;

    private ControlSettings? Control => Item.ControlSettings;

    private bool CanUse => Control != null && Count > 0;

    private string ActivateSymbol => Control == null
        ? string.Empty
        : SchemeService.CurrentScheme.GetActivateKey(Control).DisplaySymbol;

    protected override void OnParametersSet()
    {
        _sections = ParseLore(Item.Description);
    }

    private static IReadOnlyList<LoreSection> ParseLore(string description)
    {
        List<LoreSection> sections = [];
        List<string> lines = [];

        foreach (var raw in description.ReplaceLineEndings("\n").Split('\n'))
        {
            var line = raw.Trim();

            if (line.StartsWith("---", StringComparison.Ordinal))
            {
                sections.Add(BuildSection(lines));
                lines.Clear();
                continue;
            }

            if (line.Length > 0)
            {
                lines.Add(line);
            }
        }

        sections.Add(BuildSection(lines));

        return [.. sections.Where(section => section.Blocks.Count > 0)];
    }

    private static LoreSection BuildSection(List<string> lines)
    {
        List<LoreBlock> blocks = [];
        List<string> paragraph = [];
        List<string> items = [];

        foreach (var line in lines)
        {
            if (line.StartsWith("- ", StringComparison.Ordinal))
            {
                AddParagraph(blocks, paragraph);
                items.Add(line[2..]);
                continue;
            }

            AddList(blocks, items);
            paragraph.Add(line);
        }

        AddParagraph(blocks, paragraph);
        AddList(blocks, items);

        return new(blocks);
    }

    private static void AddParagraph(List<LoreBlock> blocks, List<string> paragraph)
    {
        if (paragraph.Count == 0)
        {
            return;
        }

        blocks.Add(new(string.Join(' ', paragraph), []));
        paragraph.Clear();
    }

    private static void AddList(List<LoreBlock> blocks, List<string> items)
    {
        if (items.Count == 0)
        {
            return;
        }

        blocks.Add(new(null, [.. items]));
        items.Clear();
    }

    private string? GetSectionModifier(int index)
    {
        if (index == 0)
        {
            return "lore-section--lead";
        }

        return index == _sections.Count - 1 && _sections.Count > 2
            ? "lore-section--epilogue"
            : null;
    }

    private void Use()
    {
        Instance.Close(true);
    }

    private sealed record LoreSection(IReadOnlyList<LoreBlock> Blocks);

    private sealed record LoreBlock(string? Paragraph, IReadOnlyList<string> Items);
}
