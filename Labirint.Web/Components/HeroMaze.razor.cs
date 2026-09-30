using Labirint.Web.Common.Hero;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components;

public partial class HeroMaze : IDisposable
{
    private const int CyclePause = 1200;

    private HeroRun _run = null!;
    private int? _appliedSeed;
    private bool _isDisposed;
    private CancellationTokenSource _waitTokenSource = new();

    [Parameter]
    public int Seed { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    [Inject]
    private MotionService MotionService { get; set; } = null!;

    public void Dispose()
    {
        _isDisposed = true;
        _waitTokenSource.Cancel();
        _waitTokenSource.Dispose();
    }

    protected override void OnParametersSet()
    {
        if (_appliedSeed == Seed)
        {
            return;
        }

        _appliedSeed = Seed;
        _run = HeroRunBuilder.Build(Seed);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == false || MotionService.IsReduced)
        {
            return;
        }

        await CycleAsync();
    }

    private async Task CycleAsync()
    {
        while (_isDisposed == false)
        {
            var token = _waitTokenSource.Token;

            try
            {
                await Task.Delay(_run.Duration + CyclePause, token);
            }
            catch (OperationCanceledException)
            {
                continue;
            }

            if (IsPageHidden())
            {
                continue;
            }

            ShowNext();

            await InvokeAsync(StateHasChanged);
        }
    }

    private bool IsPageHidden()
    {
        if (JSRuntime is not IJSInProcessRuntime runtime)
        {
            return false;
        }

        try
        {
            return runtime.Invoke<bool>("labirintPage.isHidden");
        }
        catch (JSException)
        {
            return false;
        }
    }

    private void Regenerate()
    {
        ShowNext();

        var previous = _waitTokenSource;
        _waitTokenSource = new CancellationTokenSource();
        previous.Cancel();
        previous.Dispose();
    }

    private void ShowNext()
    {
        _run = HeroRunBuilder.BuildNext();
    }
}
