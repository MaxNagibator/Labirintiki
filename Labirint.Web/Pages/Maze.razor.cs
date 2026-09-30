using Labirint.Core.TileFeatures;
using Labirint.Core.TileFeatures.Base;
using Labirint.Web.Common.Animation;
using Labirint.Web.Common.Ui;
using Labirint.Web.Components;
using Labirint.Web.Components.Dialogs;
using Labirint.Web.Parameters;
using Labirint.Web.Services.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Pages;

public partial class Maze : IDisposable
{
    private const int MinSize = 1;
    private const int MaxSize = 500;

    private const int MinDensity = 0;
    private const int MaxDensity = 100;

    private const int DefaultSize = 16;
    private const int DefaultDensity = 40;

    private const int AnnouncementCapacity = 4;

    private const string StepSound = "step";

    private readonly List<Announcement> _announcements = [];

    private int _announcementId;

    private bool _isInit;
    private bool _isSettingsOpen;
    private bool _isRegenerationPending;
    private bool _isGenerating;
    private bool _isCasting;
    private bool _isDisposed;

    private int _generation;
    private CancellationTokenSource? _generationCancellation;

    private string? _appliedSeed;
    private int? _appliedSize;
    private int? _appliedDensity;

    private int _originalSize;
    private int _density;

    private int _generatedSize;
    private int _generatedDensity;

    private int _displayScore;
    private int _generationProgress;

    private MazeField? _field;
    private KeyInterceptor? _keyInterceptor;
    private RunnerInventory? _runnerInventory;

    private MazeSession _session = null!;
    private RandomGenerator _seeder = null!;

    [Parameter]
    public string? Seed { get; set; }

    [SupplyParameterFromQuery(Name = RandomGenerator.SizeQueryName)]
    public string? MazeSize { get; set; }

    [SupplyParameterFromQuery(Name = RandomGenerator.DensityQueryName)]
    public string? MazeDensity { get; set; }

    [Inject]
    private SoundService SoundService { get; set; } = null!;

    [Inject]
    private LabyrinthParametersService ParametersService { get; set; } = null!;

    [Inject]
    private AnimationService AnimationService { get; set; } = null!;

    [Inject]
    private DialogService DialogService { get; set; } = null!;

    [Inject]
    private MotionService MotionService { get; set; } = null!;

    private bool IsInit => _isInit && _session is { IsReady: true };

    private bool IsBusy => IsInit == false || _isGenerating;

    private string LoadingText => _isGenerating && _generationProgress is > 0 and < 100
        ? $"Генерация.. {_generationProgress} %"
        : "Загрузка..";

    public void Dispose()
    {
        _isDisposed = true;
        DialogService.CancelOwnedBy(this);
        _generationCancellation?.Cancel();
        _generationCancellation?.Dispose();
        _generationCancellation = null;

        ParametersService.Changed -= OnLabyrinthParametersChanged;

        if (_keyInterceptor != null)
        {
            _keyInterceptor.AttackKeyDown -= OnAttackKeyDown;
            _keyInterceptor.MoveKeyDown -= OnMoveKeyDown;
        }

        if (_session != null)
        {
            _session.Finished -= OnFinished;
            _session.Labyrinth.RunnerMoved -= OnRunnerMoved;
            _session.Labyrinth.ItemPickedUp -= OnItemPickedUp;
            _session.Runner.Inventory.ItemUsed -= OnItemUsed;
            _session.Runner.ScoreIncreased -= OnScoreIncreased;
            _session.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        ParametersService.Changed += OnLabyrinthParametersChanged;
    }

    protected override void OnParametersSet()
    {
        var size = RandomGenerator.ParseQueryNumber(MazeSize);
        var density = RandomGenerator.ParseQueryNumber(MazeDensity);
        var isRouteChanged = Seed != _appliedSeed || size != _appliedSize || density != _appliedDensity;

        _appliedSeed = Seed;
        _appliedSize = size;
        _appliedDensity = density;

        _originalSize = size ?? DefaultSize;
        _density = density ?? DefaultDensity;

        if (isRouteChanged && _session != null)
        {
            _generationCancellation?.Cancel();
            _isRegenerationPending = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == false)
        {
            if (_isRegenerationPending)
            {
                _isRegenerationPending = false;
                await GenerateAsync();
            }

            return;
        }

        _session = new(_seeder.Source);
        _session.Finished += OnFinished;
        _session.Labyrinth.RunnerMoved += OnRunnerMoved;
        _session.Labyrinth.ItemPickedUp += OnItemPickedUp;
        _session.Runner.Inventory.ItemUsed += OnItemUsed;
        _session.Runner.ScoreIncreased += OnScoreIncreased;

        if (_keyInterceptor != null)
        {
            _keyInterceptor.AttackKeyDown += OnAttackKeyDown;
            _keyInterceptor.MoveKeyDown += OnMoveKeyDown;
            _keyInterceptor.InitializeItems();
        }

        await GenerateAsync();
    }

    private void OpenSettings()
    {
        _isSettingsOpen = true;
    }

    private void CloseSettings()
    {
        _isSettingsOpen = false;
    }

    private void OnSwipe(Direction direction)
    {
        _keyInterceptor?.OnKeyDown(direction);
    }

    private void OnLabyrinthParametersChanged(object? sender, EventArgs args)
    {
        if (IsInit == false || _isGenerating)
        {
            return;
        }

        RunSafe(ForceRender);
    }

    private void OnRunnerMoved(object? sender, Position args)
    {
        RunSafe(async () =>
        {
            Announce($"Ряд {args.Y + 1}, столбец {args.X + 1}");

            await ForceRender();

            SoundService.Play(StepSound);
        });
    }

    private void OnFinished(object? sender, EventArgs args)
    {
        RunSafe(ShowWinDialogAsync);
    }

    private async Task ShowWinDialogAsync()
    {
        DialogParameters parameters = new()
        {
            [nameof(WinDialog.OnRestart)] = (Func<Task>)GenerateAsync,
            [nameof(WinDialog.OnRepeat)] = (Func<Task>)RepeatAsync,
            [nameof(WinDialog.Seeder)] = _seeder,
            [nameof(WinDialog.Score)] = _session.Runner.Score,
            [nameof(WinDialog.MoveCount)] = _session.MoveCount,
            [nameof(WinDialog.Size)] = _generatedSize,
        };

        var result = await DialogService.ShowAsync<WinDialog>(this, "Финал Лабиринта", parameters, new DialogOptions
        {
            CloseButton = false,
            CloseOnBackdropClick = false,
            CloseOnEscape = false,
            Width = DialogWidth.Large,
        });

        if (result.GetValue<bool>())
        {
            _session.Continue();
        }
    }

    private void OnItemPickedUp(object? sender, TileFeature args)
    {
        RunSafe(async () =>
        {
            Announce(args is WorldItem worldItem
                ? $"Подобран предмет: {worldItem.Item.DisplayName}"
                : "Предмет подобран");

            await ForceRenderEntities();

            SoundService.Play(args.PickUpSound);
        });
    }

    private void OnScoreIncreased(object? sender, int amount)
    {
        RunSafe(async () =>
        {
            await Task.Delay(AnimatedStackExtensions.PickupFlightDuration);

            _displayScore = _session.Runner.Score;
            StateHasChanged();
        });
    }

    private void OnMoveKeyDown(object? sender, MoveEventArgs args)
    {
        if (_isCasting)
        {
            return;
        }

        Move(args.Direction);
    }

    private void OnAttackKeyDown(object? sender, AttackEventArgs args)
    {
        if (_session.IsExitFound || _isGenerating)
        {
            return;
        }

        var item = args.Item;
        var direction = args.Direction;

        if (item == null)
        {
            Move(direction);
            return;
        }

        if (_session.Runner.Inventory.CanUse(item) == false || _runnerInventory == null)
        {
            _session.Runner.UseItem(item, direction);
            Move(direction);
            return;
        }

        if (_isCasting || _runnerInventory.TryStartCast(item) == false)
        {
            return;
        }

        _isCasting = true;

        var generation = _generation;
        var flightDuration = MotionService.IsReduced ? 0 : AnimatedStackExtensions.UseFlightDuration;

        RunSafe(async () =>
        {
            try
            {
                await Task.Delay(flightDuration);

                if (generation != _generation || _session.IsExitFound || (_keyInterceptor?.IsPaused ?? false))
                {
                    _runnerInventory.CancelCast(item);
                    return;
                }

                _session.Runner.UseItem(item, direction);
                Move(direction);
            }
            finally
            {
                _isCasting = false;
            }
        });
    }

    private void Move(Direction? direction)
    {
        if (direction is null or Direction.None || _session.IsExitFound || _isGenerating)
        {
            return;
        }

        _session.Labyrinth.Move(direction.Value);
    }

    private void OnItemUsed(object? sender, Item item)
    {
        RunSafe(async () =>
        {
            Announce($"Использован предмет: {item.DisplayName}");

            await ForceRender();
            SoundService.Play(item.SoundSettings?.UseSound);
        });
    }

    private Task GenerateAsync()
    {
        return GenerateAsync(false);
    }

    private Task RepeatAsync()
    {
        return GenerateAsync(true);
    }

    private async Task GenerateAsync(bool isSeedRepeated)
    {
        if (_isDisposed)
        {
            return;
        }

        _generationCancellation?.Cancel();
        _generationCancellation?.Dispose();
        _generationCancellation = new();

        var cancellationToken = _generationCancellation.Token;

        AnimationService.StartRandomAnimationEffect();

        _generation++;
        _keyInterceptor?.ResetWaitItem();
        _announcements.Clear();

        _isGenerating = true;
        _generationProgress = 0;
        StateHasChanged();

        try
        {
            await Task.Delay(1, cancellationToken);

            if (isSeedRepeated)
            {
                _seeder.Repeat();
            }
            else
            {
                _seeder.Reload();
            }

            _displayScore = 0;

            _originalSize = Math.Clamp(_originalSize, MinSize, MaxSize);
            _density = Math.Clamp(_density, MinDensity, MaxDensity);

            _generatedSize = _originalSize;
            _generatedDensity = _density;

            var progress = new Progress<int>(percent => OnGenerationProgress(percent, cancellationToken));
            await _session.GenerateAsync(_generatedSize, _generatedDensity, progress, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        _isGenerating = false;

        ReplaceRouteSeed();

        StateHasChanged();

        await ForceRender();

        _isInit = true;
        StateHasChanged();
    }

    private void ReplaceRouteSeed()
    {
        var seed = _seeder.RouteSeed;

        if (seed == _appliedSeed && _generatedSize == _appliedSize && _generatedDensity == _appliedDensity)
        {
            return;
        }

        _appliedSeed = seed;
        _appliedSize = _generatedSize;
        _appliedDensity = _generatedDensity;

        _seeder.ReplaceRoute(seed, _generatedSize, _generatedDensity);
    }

    private async Task ForceRender()
    {
        await (_field?.ForceRenderAsync() ?? Task.CompletedTask);

        StateHasChanged();
    }

    private async Task ForceRenderEntities()
    {
        await (_field?.ForceRenderEntitiesAsync() ?? Task.CompletedTask);

        StateHasChanged();
    }

    private void OnGenerationProgress(int percent, CancellationToken cancellationToken)
    {
        if (_isGenerating == false || cancellationToken.IsCancellationRequested)
        {
            return;
        }

        _generationProgress = percent;
        StateHasChanged();
    }

    private void Announce(string text)
    {
        _announcements.Add(new(++_announcementId, text));

        if (_announcements.Count > AnnouncementCapacity)
        {
            _announcements.RemoveAt(0);
        }
    }

    private sealed record Announcement(int Id, string Text);
}
