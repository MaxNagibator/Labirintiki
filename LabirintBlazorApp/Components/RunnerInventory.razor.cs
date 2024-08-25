using LabirintBlazorApp.Common.Control.Schemes;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class RunnerInventory : IAsyncDisposable
{
    [Parameter]
    public required Inventory Inventory { get; set; }

    [Parameter]
    public required KeyInterceptor Interceptor { get; set; }

    [Inject]
    public required IControlSchemeService SchemeService { get; set; }

    private Item? WaitItem { get; set; }

    private IControlScheme ControlScheme => SchemeService.CurrentScheme;

    public async ValueTask DisposeAsync()
    {
        Interceptor.ChangedWaitItem -= OnChangedWaitItem;
        SchemeService.ControlSchemeChanged -= OnSchemeChanged;
        await Interceptor.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected override void OnParametersSet()
    {
        Interceptor.ChangedWaitItem += OnChangedWaitItem;
        SchemeService.ControlSchemeChanged += OnSchemeChanged;
    }

    private void OnSchemeChanged(object? sender, IControlScheme scheme)
    {
        StateHasChanged();
    }

    private void OnChangedWaitItem(object? sender, Item? args)
    {
        WaitItem = args;
        StateHasChanged();
    }

    private void OnClicked(Key activateKey)
    {
        Interceptor.OnKeyDown(activateKey);
    }
}
