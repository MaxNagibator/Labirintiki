using Labirint.Web.Common.Ui;
using Labirint.Web.Components.Dialogs;
using Labirint.Web.Services;
using Labirint.Web.Services.Dialogs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace Labirint.Web.Layout;

public partial class MainLayout
{
    private ErrorBoundary? _errorBoundary;

    [Inject]
    public required ThemeService ThemeService { get; set; }

    [Inject]
    public required DialogService DialogService { get; set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Inject]
    public required ILogger<MainLayout> Logger { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await ThemeService.InitializeAsync();
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Не удалось применить тему, остаётся тема по умолчанию");
        }

    }

    protected override void OnParametersSet()
    {
        _errorBoundary?.Recover();
    }

    private void Reload()
    {
        NavigationManager.Refresh(true);
    }

    private Task ToggleThemeAsync()
    {
        return ThemeService.ToggleAsync();
    }

    private Task OpenParametersAsync()
    {
        return DialogService.ShowAsync<OpenParametersDialog>(this, "Настройки", options: new DialogOptions
        {
            CloseOnBackdropClick = false,
            Width = DialogWidth.Small,
        });
    }
}
