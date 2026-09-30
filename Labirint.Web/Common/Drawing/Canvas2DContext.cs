namespace Labirint.Web.Common.Drawing;

public class Canvas2DContext(IJSInProcessObjectReference context, IJSInProcessRuntime jsRuntime) : IDisposable
{
    public void Draw(DrawSequence sequence)
    {
        jsRuntime.InvokeVoid("canvasHelper.drawCommands", context, sequence.Build());
    }

    public void Dispose()
    {
        context.Dispose();
    }
}
