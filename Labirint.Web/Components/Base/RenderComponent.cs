namespace Labirint.Web.Components.Base;

public abstract class RenderComponent : SafeComponent
{
    private bool _isRenderRequested;

    public async Task ForceRenderAsync()
    {
        await OnRenderAsyncInner();

        _isRenderRequested = true;
        StateHasChanged();
    }

    protected sealed override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await OnFirstRenderAsyncInner();
            await ForceRenderAsync();
        }

        await OnAfterRenderAsyncInner(firstRender);
    }

    protected virtual Task OnAfterRenderAsyncInner(bool firstRender)
    {
        return Task.CompletedTask;
    }

    protected sealed override bool ShouldRender()
    {
        if (_isRenderRequested == false)
        {
            return false;
        }

        _isRenderRequested = false;
        return true;
    }

    protected abstract Task OnRenderAsyncInner();
    protected abstract Task OnFirstRenderAsyncInner();
}
