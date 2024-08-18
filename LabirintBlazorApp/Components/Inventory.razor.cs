using System.Collections.ObjectModel;
using Labirint.Core.Common;
using Labirint.Core.Stacks;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class Inventory : IAsyncDisposable
{
    [Parameter]
    public required ObservableCollection<ItemStack> Stacks { get; set; }

    [Parameter]
    public required KeyInterceptor Interceptor { get; set; }

    private Item? WaitItem { get; set; }

    public async ValueTask DisposeAsync()
    {
        Interceptor.ChangedWaitItem -= OnChangedWaitItem;
        await Interceptor.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected override void OnParametersSet()
    {
        Interceptor.ChangedWaitItem += OnChangedWaitItem;
    }

    private void OnChangedWaitItem(object? sender, Item? args)
    {
        WaitItem = args;
        StateHasChanged();
    }

    private async Task OnClicked(Key activateKey)
    {
        await Interceptor.OnKeyDown(activateKey);
    }
}
