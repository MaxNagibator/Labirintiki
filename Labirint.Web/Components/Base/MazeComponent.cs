using Labirint.Web.Parameters;
using Labirint.Web.Services;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components.Base;

public abstract class MazeComponent : RenderComponent, IDisposable
{
    protected ElementReference CanvasRef;

    private Canvas2DContext? _context;

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Inject]
    public required LabyrinthParametersService ParametersService { get; set; }

    [CascadingParameter]
    public required MazeRenderParameters RenderParameters { get; set; }

    public int CanvasWidth { get; private set; }
    public int CanvasHeight { get; private set; }

    protected virtual string FillStyle => ParametersService.Current.Color;

    protected int BoxSize { get; private set; }
    protected int WallWidth { get; private set; }
    protected Labyrinth Maze { get; private set; } = null!;
    protected Vision Vision { get; private set; } = null!;

    protected abstract string CanvasId { get; }

    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void OnParametersSetInner()
    {
    }

    protected sealed override void OnParametersSet()
    {
        (Maze, BoxSize, WallWidth, Vision) = RenderParameters;

        var renderRange = RenderParameters.RenderRange;
        CanvasWidth = renderRange;
        CanvasHeight = renderRange;

        OnParametersSetInner();
    }

    protected override Task OnRenderAsyncInner()
    {
        DrawSequence drawSequence = new();
        drawSequence.ClearRect(0, 0, CanvasWidth, CanvasHeight);
        drawSequence.FillStyle(FillStyle);

        for (var x = Vision.Start.X; x <= Vision.Finish.X; x++)
        {
            for (var y = Vision.Start.Y; y <= Vision.Finish.Y; y++)
            {
                DrawInner(x, y, drawSequence);
            }
        }

        _context?.Draw(drawSequence);

        return Task.CompletedTask;
    }

    protected override Task OnFirstRenderAsyncInner()
    {
        var runtime = (IJSInProcessRuntime)JSRuntime;
        var contextRef = runtime.Invoke<IJSInProcessObjectReference?>("canvasHelper.getContext2D", CanvasRef);

        _context = contextRef == null ? null : new(contextRef, runtime);

        return Task.CompletedTask;
    }

    protected abstract void DrawInner(int x, int y, DrawSequence sequence);
}
