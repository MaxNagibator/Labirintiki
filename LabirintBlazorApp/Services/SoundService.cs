using Microsoft.JSInterop;

namespace LabirintBlazorApp.Services;

public class SoundService(IJSRuntime jsRuntime)
{
    public ValueTask PlayAsync(string soundType)
    {
        if (Parameters.Labyrinth.IsSoundOn == false)
        {
            return ValueTask.CompletedTask;
        }

        return jsRuntime.InvokeVoidAsync("playSound", soundType);
    }
}
