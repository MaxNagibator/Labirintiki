using Labirint.Web.Common.Ui;
using Labirint.Web.Services.Dialogs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace Labirint.Web.Components.Ui;

public partial class DialogHost : IDisposable
{
    private readonly Dictionary<Guid, ElementReference> _panels = [];
    private readonly List<Guid> _trapped = [];

    [Inject]
    public required DialogService DialogService { get; set; }

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Inject]
    public required ILogger<DialogHost> Logger { get; set; }

    public void Dispose()
    {
        DialogService.Changed -= OnChanged;
    }

    protected override void OnInitialized()
    {
        DialogService.Changed += OnChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        List<Guid> shown = DialogService.Instances.Select(instance => instance.Id).ToList();

        for (int index = _trapped.Count - 1; index >= 0; index--)
        {
            Guid id = _trapped[index];

            if (shown.Contains(id))
            {
                continue;
            }

            _trapped.RemoveAt(index);
            _panels.Remove(id);

            await ReleaseAsync(id);
        }

        foreach (Guid id in shown)
        {
            if (_trapped.Contains(id) || _panels.TryGetValue(id, out ElementReference panel) == false)
            {
                continue;
            }

            _trapped.Add(id);
            await JSRuntime.InvokeVoidAsync("labirintDialog.trap", panel, GetLayerKey(id));
        }
    }

    private static string GetLayerKey(Guid id)
    {
        return $"dialog-{id:N}";
    }

    private async ValueTask ReleaseAsync(Guid id)
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("labirintDialog.release", GetLayerKey(id));
        }
        catch (JSException exception)
        {
            Logger.LogError(exception, "Не удалось снять ловушку фокуса диалога");
        }
    }

    private static string GetWidthClass(DialogInstance instance)
    {
        return instance.Options.Width switch
        {
            DialogWidth.Small => "ui-dialog-panel--small",
            DialogWidth.Large => "ui-dialog-panel--large",
            DialogWidth.Medium => "ui-dialog-panel--medium",
            _ => "ui-dialog-panel--auto",
        };
    }

    private void OnChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void OnBackdropClick()
    {
        DialogInstance? topmost = GetTopmost();

        if (topmost?.Options.CloseOnBackdropClick == true)
        {
            topmost.Cancel();
        }
    }

    private void OnKeyDown(KeyboardEventArgs args)
    {
        if (args.Key != "Escape")
        {
            return;
        }

        DialogInstance? topmost = GetTopmost();

        if (topmost?.Options.CloseOnEscape == true)
        {
            topmost.Cancel();
        }
    }

    private DialogInstance? GetTopmost()
    {
        return DialogService.Instances.Count > 0
            ? DialogService.Instances[^1]
            : null;
    }
}
