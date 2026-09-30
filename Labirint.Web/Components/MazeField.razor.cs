using Labirint.Web.Parameters;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components;

public partial class MazeField : SafeComponent, IDisposable
{
    private const double RunnerScale = 0.9;

    private MazeFloor? _mazeFloor;
    private MazeWalls? _mazeWalls;
    private MazeEntities? _mazeEntities;
    private TouchInterceptor? _touchInterceptor;

    private int _runnerScaleX = 1;

    [Parameter]
    [EditorRequired]
    public required MazeRenderParameters Parameters { get; set; }

    [Parameter]
    public string? AnimationClass { get; set; }

    [Parameter]
    public EventCallback<Direction> OnSwipe { get; set; }

    public void Dispose()
    {
        if (_touchInterceptor != null)
        {
            _touchInterceptor.Moved -= OnMoved;
        }

        GC.SuppressFinalize(this);
    }

    public Task ForceRenderAsync()
    {
        return Task.WhenAll(_mazeFloor?.ForceRenderAsync() ?? Task.CompletedTask,
            _mazeWalls?.ForceRenderAsync() ?? Task.CompletedTask,
            _mazeEntities?.ForceRenderAsync() ?? Task.CompletedTask);
    }

    public Task ForceRenderEntitiesAsync()
    {
        return _mazeEntities?.ForceRenderAsync() ?? Task.CompletedTask;
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _touchInterceptor != null)
        {
            _touchInterceptor.Moved += OnMoved;
        }

        return Task.CompletedTask;
    }

    private void OnMoved(object? sender, Direction direction)
    {
        RunSafe(() => OnSwipe.InvokeAsync(direction));
    }
}
