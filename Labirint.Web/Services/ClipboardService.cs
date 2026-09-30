namespace Labirint.Web.Services;

public class ClipboardService(IJSRuntime jsRuntime)
{
    public async ValueTask<bool> CopyToClipboardAsync(string text)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
            return true;
        }
        catch (JSException)
        {
            return false;
        }
    }
}
