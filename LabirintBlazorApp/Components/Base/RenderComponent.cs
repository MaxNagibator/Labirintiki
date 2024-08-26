using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components.Base;

public abstract class RenderComponent : ComponentBase
{
    private bool _isShouldRender;

    public async Task ForceRenderAsync()
    {
        _isShouldRender = true;

        await OnRenderAsyncInner();
        StateHasChanged();

        _isShouldRender = false;
    }

    protected sealed override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await OnFirstRenderAsyncInner();
            await ForceRenderAsync();
        }
    }

    protected sealed override bool ShouldRender()
    {
        return _isShouldRender;
    }

    protected abstract Task OnRenderAsyncInner();
    protected abstract Task OnFirstRenderAsyncInner();
}
