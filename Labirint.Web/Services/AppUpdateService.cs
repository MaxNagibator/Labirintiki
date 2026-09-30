namespace Labirint.Web.Services;

public sealed class AppUpdateService(IJSRuntime jsRuntime) : IDisposable
{
    private DotNetObjectReference<AppUpdateService>? _reference;

    public event Action? UpdateReady;

    public bool IsUpdateReady { get; private set; }

    public ValueTask WatchAsync()
    {
        _reference ??= DotNetObjectReference.Create(this);
        return jsRuntime.InvokeVoidAsync("labirintUpdate.watch", _reference);
    }

    public ValueTask ApplyAsync()
    {
        return jsRuntime.InvokeVoidAsync("labirintUpdate.apply");
    }

    [JSInvokable]
    public void OnUpdateReady()
    {
        IsUpdateReady = true;
        UpdateReady?.Invoke();
    }

    public void Dispose()
    {
        _reference?.Dispose();
    }
}
