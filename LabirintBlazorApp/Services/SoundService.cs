using LabirintBlazorApp.Parameters;

namespace LabirintBlazorApp.Services;

public class SoundService(IJSRuntime jsRuntime)
{
    public ValueTask PlayAsync(string soundType)
    {
        if (GlobalParameters.Labyrinth.IsSoundOn == false)
        {
            return ValueTask.CompletedTask;
        }

        return jsRuntime.InvokeVoidAsync("playSound", soundType, GlobalParameters.Labyrinth.SoundVolume);
    }
}
