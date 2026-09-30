using Labirint.Web.Common.Ui;
using Labirint.Web.Services.Toasts;
using Microsoft.Extensions.Logging;

namespace Labirint.Web.Services;

public sealed class ThemeService(IJSRuntime jsRuntime, LocalStorageService localStorage, ToastService toastService, ILogger<ThemeService> logger)
{
    private const string StorageKey = "IsDarkMod";

    public bool IsDark { get; private set; } = true;

    public async Task InitializeAsync()
    {
        bool? isDark = null;

        try
        {
            isDark = await localStorage.GetItemAsync<bool?>(StorageKey);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Не удалось прочитать выбранную тему, берётся системная");
        }

        IsDark = isDark ?? await jsRuntime.InvokeAsync<bool>("labirintTheme.isSystemDark");
        await ApplyAsync();
    }

    public async Task ToggleAsync()
    {
        IsDark = IsDark == false;

        try
        {
            await localStorage.SetItemAsync(StorageKey, IsDark);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Не удалось сохранить выбранную тему");
            toastService.Show("Не удалось запомнить тему", UiSeverity.Warning);
        }

        await ApplyAsync();
    }

    private ValueTask ApplyAsync()
    {
        return jsRuntime.InvokeVoidAsync("labirintTheme.apply", IsDark ? "dark" : "light");
    }
}
