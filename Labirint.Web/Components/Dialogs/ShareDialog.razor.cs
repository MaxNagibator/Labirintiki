using Labirint.Web.Services.Dialogs;
using Labirint.Web.Services.Toasts;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components.Dialogs;

public partial class ShareDialog
{
    private bool _isShowMotivation;

    [Parameter]
    [EditorRequired]
    public required string Link { get; set; }

    [Parameter]
    public string? UserSeed { get; set; }

    [Parameter]
    public bool IsExitFound { get; set; }

    [CascadingParameter]
    private DialogInstance Instance { get; set; } = null!;

    [Inject]
    private ClipboardService ClipboardService { get; set; } = null!;

    [Inject]
    private ToastService ToastService { get; set; } = null!;

    private void ToggleMotivation()
    {
        _isShowMotivation = _isShowMotivation == false;
    }

    private async Task CopyLinkAsync()
    {
        if (await ClipboardService.CopyToClipboardAsync(Link))
        {
            ToastService.Show("Ссылка скопирована!", UiSeverity.Success);
            return;
        }

        ToastService.Show("Не вышло скопировать, выдели ссылку и скопируй вручную", UiSeverity.Warning);
    }
}
