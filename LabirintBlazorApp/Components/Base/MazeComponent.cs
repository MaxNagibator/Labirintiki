using LabirintBlazorApp.Common.Drawing;
using LabirintBlazorApp.Parameters;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components.Base;

public abstract class MazeComponent : ComponentBase
{
    private bool _isShouldRender;

    private Canvas2DContext _context = null!;
    protected ElementReference CanvasRef;

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Inject]
    public required ILogger<MazeComponent> Logger { get; set; }

    [CascadingParameter]
    public required MazeRenderParameter MazeRenderParameter { get; set; }

    protected int BoxSize { get; private set; }
    protected int WallWidth { get; private set; }
    protected Labyrinth Maze { get; private set; } = null!;
    protected Vision Vision { get; private set; } = null!;

    protected int CanvasWidth { get; private set; }
    protected int CanvasHeight { get; private set; }

    protected abstract string CanvasId { get; }

    protected sealed override void OnParametersSet()
    {
        (Maze, BoxSize, WallWidth, Vision) = MazeRenderParameter;

        // Данное замечание актуально, если в MazeWalls оставлять условия с исключением повторного рисования стен
        // и необходимо заменить перед прочтением 50 строку на данную: int renderRange = Vision.Range * 2 * BoxSize + WallWidth;
        // По факту рисуется Vision.Range * 2 * BoxSize + BoxSize + WallWidth,
        // но чтобы было (возможно) красивее оставлена только верхнюю часть ячейки (картинки были в предыдущем PR).
        // Из-за этого игрок размещается не в центре радиуса видимости.
        // Если данное поведение не устраивает, нужно заменить стоку 50 на закомментированную ниже.
        // int renderRange = Vision.Range * 2 * BoxSize + BoxSize + WallWidth;

        // Vision.Range * 2 -> область видимости во все стороны.
        // * BoxSize -> из относительного в абсолютное значение.
        // + BoxSize -> клетка с игроком.
        // + WallWidth -> для того, чтобы видеть стенки у клеток на границе обзора.

        int renderRange = Vision.Range * 2 * BoxSize + BoxSize + WallWidth;
        CanvasWidth = renderRange;
        CanvasHeight = renderRange;

        OnParametersSetInner();
    }

    protected virtual void OnParametersSetInner()
    {
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitAsync();
            await ForceRender();
        }
    }

    protected override bool ShouldRender()
    {
        return _isShouldRender;
    }

    public async Task ForceRender()
    {
        _isShouldRender = true;

        await DrawAsync();
        StateHasChanged();

        _isShouldRender = false;
    }

    protected abstract void DrawInner(int x, int y, DrawSequence sequence);

    private async Task DrawAsync()
    {
        DrawSequence drawSequence = new();
        drawSequence.ClearRect(0, 0, CanvasWidth, CanvasHeight);
        drawSequence.StrokeStyle(GlobalParameters.Labyrinth.Color);

        for (int x = Vision.Start.X; x <= Vision.Finish.X; x++)
        {
            for (int y = Vision.Start.Y; y <= Vision.Finish.Y; y++)
            {
                DrawInner(x, y, drawSequence);
            }
        }

        await _context.DrawSequenceAsync(drawSequence);
    }

    private async Task InitAsync()
    {
        IJSObjectReference contextRef = await JSRuntime.InvokeAsync<IJSObjectReference>("canvasHelper.getContext2D", CanvasRef);
        _context = new Canvas2DContext(contextRef, JSRuntime);
    }
}
