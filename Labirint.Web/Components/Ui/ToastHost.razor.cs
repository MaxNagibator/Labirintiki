using Labirint.Web.Common.Ui;
using Labirint.Web.Services.Toasts;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components.Ui;

public partial class ToastHost : IDisposable
{
    [Inject]
    public required ToastService ToastService { get; set; }

    public void Dispose()
    {
        ToastService.Changed -= OnChanged;
    }

    protected override void OnInitialized()
    {
        ToastService.Changed += OnChanged;
    }

    private static string GetSeverityClass(Toast toast)
    {
        return toast.Severity switch
        {
            UiSeverity.Success => "ui-alert--success",
            UiSeverity.Warning => "ui-alert--warning",
            UiSeverity.Error => "ui-alert--error",
            _ => "ui-alert--info",
        };
    }

    private static string GetIcon(Toast toast)
    {
        return toast.Severity switch
        {
            UiSeverity.Success => IconCatalog.Check,
            UiSeverity.Warning => IconCatalog.Warning,
            UiSeverity.Error => IconCatalog.Error,
            _ => IconCatalog.Info,
        };
    }

    private void OnChanged()
    {
        InvokeAsync(StateHasChanged);
    }
}
