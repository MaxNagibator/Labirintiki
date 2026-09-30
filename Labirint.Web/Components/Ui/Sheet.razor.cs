using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace Labirint.Web.Components.Ui;

public partial class Sheet : IAsyncDisposable
{
    private readonly string _layerKey = $"sheet-{Guid.NewGuid():N}";

    private ElementReference _sheet;
    private bool _isTrapped;

    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    [EditorRequired]
    public required string Title { get; set; }

    [Parameter]
    public string CloseTitle { get; set; } = "Закрыть";

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    [Inject]
    private ILogger<Sheet> Logger { get; set; } = null!;

    public async ValueTask DisposeAsync()
    {
        if (_isTrapped)
        {
            _isTrapped = false;
            await ReleaseAsync();
        }

        GC.SuppressFinalize(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || _isTrapped == IsOpen)
        {
            return;
        }

        _isTrapped = IsOpen;

        await (IsOpen
            ? JSRuntime.InvokeVoidAsync("labirintDialog.trap", _sheet, _layerKey)
            : ReleaseAsync());
    }

    private async ValueTask ReleaseAsync()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("labirintDialog.release", _layerKey);
        }
        catch (JSException exception)
        {
            Logger.LogError(exception, "Не удалось снять ловушку фокуса шторки");
        }
    }

    private Task CloseAsync()
    {
        return IsOpen == false ? Task.CompletedTask : OnClose.InvokeAsync();
    }

    private Task OnKeyDown(KeyboardEventArgs args)
    {
        return args.Key == "Escape" ? CloseAsync() : Task.CompletedTask;
    }
}
