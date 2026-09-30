namespace Labirint.Web.Services;

public class SoundService(IJSRuntime jsRuntime, LabyrinthParametersService parametersService)
{
    public void Play(string? soundType)
    {
        if (string.IsNullOrWhiteSpace(soundType) || parametersService.Current.IsSoundOn == false)
        {
            return;
        }

        try
        {
            ((IJSInProcessRuntime)jsRuntime).InvokeVoid("playSound", soundType, parametersService.Current.SoundVolume);
        }
        catch (JSException)
        {
        }
    }
}
