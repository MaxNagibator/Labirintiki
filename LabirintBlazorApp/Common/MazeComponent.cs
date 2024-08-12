using LabirintBlazorApp.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LabirintBlazorApp.Common;

public abstract class MazeComponent : ComponentBase
{
    private bool _isShouldRender;

    protected Canvas2DContext Context = null!;
    protected ElementReference CanvasRef;

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Inject]
    public required ILogger<MazeComponent> Logger { get; set; }

    [Parameter]
    [EditorRequired]
    public required int BoxSize { get; set; }

    [Parameter]
    [EditorRequired]
    public required int WallWidth { get; set; }

    [Parameter]
    [EditorRequired]
    public required Labyrinth Maze { get; set; }

    [Parameter]
    [EditorRequired]
    public required Vision Vision { get; set; }

    protected int CanvasWidth { get; private set; }
    protected int CanvasHeight { get; private set; }

    protected abstract string CanvasId { get; }

    protected sealed override void OnParametersSet()
    {
        // Данное замечание актуально, если в MazeWalls оставлять условия с исключением повторного рисования стен
        // и необходимо заменить перед прочтением 56 строку на данную: int renderRange = Vision.Range * 2 * BoxSize + WallWidth;
        // По факту рисуется Vision.Range * 2 * BoxSize + BoxSize + WallWidth,
        // но чтобы было (возможно) красивее оставлена только верхнюю часть ячейки (картинки были в предыдущем PR).
        // Из-за этого игрок размещается не в центре радиуса видимости.
        // Если данное поведение не устраивает, нужно заменить стоку 56 на закомментированную ниже.
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

    protected abstract Task DrawAsync();

    private async Task InitAsync()
    {
        IJSObjectReference contextRef = await JSRuntime.InvokeAsync<IJSObjectReference>("canvasHelper.getContext2D", CanvasRef);
        Context = new Canvas2DContext(contextRef, JSRuntime);
    }
}
