using Microsoft.JSInterop;

namespace LabirintBlazorApp.Common;

public class Canvas2DContext(IJSObjectReference context, IJSRuntime jsRuntime)
{
    public ValueTask DrawSequenceAsync(DrawSequence sequence)
    {
        return jsRuntime.InvokeVoidAsync("canvasHelper.drawCommands", context, sequence.ToList());
    }
}
