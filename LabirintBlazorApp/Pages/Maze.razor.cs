using LabirintBlazorApp.Components;
using LabirintBlazorApp.Parameters;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Pages;

public partial class Maze : IAsyncDisposable
{
    private const int MinSize = 1;
    private const int MaxSize = 500;

    private bool _isExitFound;
    private bool _isInit;

    private int _originalSize;
    private int _density;

    private int _boxSize;
    private int _wallWidth;

    private MazeWalls? _mazeWalls;
    private MazeEntities? _mazeSands;
    private MazeRenderParameters? _renderParameter;
    private KeyInterceptor? _keyInterceptor;

    private Labyrinth _labyrinth = null!;
    private MazeSeed _seeder = null!;
    private Vision _vision = null!;

    [Inject]
    public required InventoryService InventoryService { get; set; }

    [Parameter]
    public string? Seed { get; set; }

    [SupplyParameterFromQuery(Name = MazeSeed.SizeQueryName)]
    public int? MazeSize { get; set; }

    [SupplyParameterFromQuery(Name = MazeSeed.DensityQueryName)]
    public int? MazeDensity { get; set; }

    // Проверка на null и инициализацию (дополнительная проверка, если флаг выставили в true, а значение у не null полей не выставили)
    private bool IsInit => _isInit && _labyrinth != null && _seeder != null && _vision != null && _renderParameter != null;

    public async ValueTask DisposeAsync()
    {
        if (_keyInterceptor != null)
        {
            await _keyInterceptor.DisposeAsync();
            _keyInterceptor.AttackKeyDown -= OnAttackKeyDown;
            _keyInterceptor.MoveKeyDown -= OnMoveKeyDown;
        }

        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        _boxSize = 48;
        _wallWidth = Math.Max(1, _boxSize / 10);
    }

    protected override void OnParametersSet()
    {
        _originalSize = MazeSize ?? 16;
        _density = MazeDensity ?? 40;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GenerateAsync();

            if (_keyInterceptor != null)
            {
                _keyInterceptor.AttackKeyDown += OnAttackKeyDown;
                _keyInterceptor.MoveKeyDown += OnMoveKeyDown;
            }
        }
    }

    private async void OnPlayerMoved(object? sender, Position args)
    {
        // TODO подумать как вынести строку
        await SoundService.PlayAsync("step");
    }

    private void OnExitFound(object? sender, EventArgs args)
    {
        _isExitFound = true;
    }

    private async void OnItemPickedUp(object? sender, WorldItem args)
    {
        await SoundService.PlayAsync(args.PickUpSound);
    }

    private async void OnMoveKeyDown(object? sender, MoveEventArgs args)
    {
        if (_isExitFound)
        {
            return;
        }

        _labyrinth.Move(args.Direction);
        _vision.SetPosition(_labyrinth.Player);

        await Task.WhenAll(ForceRenderWalls(), ForceRenderSands());
        StateHasChanged();
    }

    private async void OnAttackKeyDown(object? sender, AttackEventArgs args)
    {
        Item? item = args.Item;

        if (item != null && item.TryUse(_labyrinth.Player, args.Direction, _labyrinth))
        {
            await SoundService.PlayAsync(item.SoundSettings.UseSound);

            await ForceRenderWalls();
            StateHasChanged();
        }
    }

    private async Task GenerateAsync()
    {
        AnimationService.StartRandomAnimationEffect();
        // todo Костыль чтоб цвет обновлялся, надо больше времени подумать.
        // (Не перерисовывает если стена осталась на прежнем месте)
        // Но в принципе то работает)))))) 
        await Task.Delay(1);

        InventoryService.Clear();
        _seeder.Reload();

        if (_labyrinth != null)
        {
            _labyrinth.PlayerMoved -= OnPlayerMoved;
            _labyrinth.ExitFound -= OnExitFound;
            _labyrinth.ItemPickedUp -= OnItemPickedUp;
        }

        _isExitFound = false;

        _originalSize = Math.Max(MinSize, Math.Min(MaxSize, _originalSize));

        _labyrinth = new Labyrinth(_seeder);
        _labyrinth.Init(_originalSize, _originalSize, _density, InventoryService.Items);

        _labyrinth.PlayerMoved += OnPlayerMoved;
        _labyrinth.ExitFound += OnExitFound;
        _labyrinth.ItemPickedUp += OnItemPickedUp;

        _vision = new Vision(_originalSize, _originalSize);
        _vision.SetPosition(_labyrinth.Player);

        _renderParameter = new MazeRenderParameters(_labyrinth, _boxSize, _wallWidth, _vision);

        StateHasChanged();

        await Task.WhenAll(ForceRenderWalls(), ForceRenderSands());

        _isInit = true;
        StateHasChanged();
    }

    private Task ForceRenderSands()
    {
        return _mazeSands?.ForceRender() ?? Task.CompletedTask;
    }

    private Task ForceRenderWalls()
    {
        return _mazeWalls?.ForceRender() ?? Task.CompletedTask;
    }
}
