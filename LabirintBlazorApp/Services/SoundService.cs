using Microsoft.JSInterop;

namespace LabirintBlazorApp.Services;

public class SoundService
{
    private readonly IJSRuntime _jsRuntime;
    public SoundService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    public async Task PlayAsync(string soundType)
    {
        if (Parameters.Labyrinth.IsSoundOn)
            await _jsRuntime.InvokeVoidAsync("playSound", soundType);
    }
}
