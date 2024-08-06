using Microsoft.JSInterop;

namespace LabirintBlazorApp.Common;

public class Canvas2DContext(IJSObjectReference context, IJSRuntime jsRuntime)
{
    public ValueTask SetStrokeStyleAsync(string color)
    {
        return jsRuntime.InvokeVoidAsync("canvasHelper.setStrokeStyle", context, color);
    }

    public ValueTask SetLineWidthAsync(double wallWidth)
    {
        return jsRuntime.InvokeVoidAsync("canvasHelper.setLineWidth", context, wallWidth);
    }

    public ValueTask DrawCommandsAsync(List<DrawCommand> commands)
    {
        return jsRuntime.InvokeVoidAsync("canvasHelper.drawCommands", context, commands);
    }

    public ValueTask BeginPathAsync()
    {
        return context.InvokeVoidAsync("beginPath");
    }

    public ValueTask MoveToAsync(double x, double y)
    {
        return context.InvokeVoidAsync("moveTo", x, y);
    }

    public ValueTask LineToAsync(double x, double y)
    {
        return context.InvokeVoidAsync("lineTo", x, y);
    }

    public ValueTask StrokeAsync()
    {
        return context.InvokeVoidAsync("stroke");
    }

    public ValueTask ClearRectAsync(double x, double y, double width, double height)
    {
        return context.InvokeVoidAsync("clearRect", x, y, width, height);
    }

    public ValueTask FillRectAsync(double x, double y, double width, double height)
    {
        return context.InvokeVoidAsync("fillRect", x, y, width, height);
    }
}
