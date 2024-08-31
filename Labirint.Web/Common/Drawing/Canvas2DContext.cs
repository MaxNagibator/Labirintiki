namespace Labirint.Web.Common.Drawing;

public class Canvas2DContext(IJSObjectReference context, IJSRuntime jsRuntime)
{
    public ValueTask DrawSequenceAsync(DrawSequence sequence)
    {
        return jsRuntime.InvokeVoidAsync("canvasHelper.drawCommands", context, sequence.ToList());
    }
}
