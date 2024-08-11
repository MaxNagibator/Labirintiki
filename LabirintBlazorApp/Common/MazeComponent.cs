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
    public required int HalfBoxSize { get; set; }

    [Parameter]
    [EditorRequired]
    public required int WallWidth { get; set; }

    [Parameter]
    [EditorRequired]
    public required Labyrinth Maze { get; set; }

    [Parameter]
    [EditorRequired]
    public required Vision Vision { get; set; }

    protected int CanvasWidth => Vision.Range * 2 * HalfBoxSize + WallWidth;
    protected int CanvasHeight => Vision.Range * 2 * HalfBoxSize + WallWidth;

    protected abstract string CanvasId { get; }

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
