using Labirint.Web.Components.Base;
using Labirint.Web.Services;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components;

public partial class UpdateBanner : SafeComponent, IDisposable
{
    private bool _isDismissed;

    [Inject]
    public required AppUpdateService AppUpdate { get; set; }

    private bool IsVisible => AppUpdate.IsUpdateReady && _isDismissed == false;

    public void Dispose()
    {
        AppUpdate.UpdateReady -= OnUpdateReady;
    }

    protected override void OnInitialized()
    {
        AppUpdate.UpdateReady += OnUpdateReady;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await AppUpdate.WatchAsync();
        }
    }

    private void OnUpdateReady()
    {
        _isDismissed = false;
        InvokeAsync(StateHasChanged);
    }

    private void Apply()
    {
        RunSafe(() => AppUpdate.ApplyAsync().AsTask());
    }

    private void Dismiss()
    {
        _isDismissed = true;
    }
}
