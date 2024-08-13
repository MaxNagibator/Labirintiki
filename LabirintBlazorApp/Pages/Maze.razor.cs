using LabirintBlazorApp.Common.Control;
using LabirintBlazorApp.Common.Drawing;
using LabirintBlazorApp.Components;
using LabirintBlazorApp.Constants;
using LabirintBlazorApp.Parameters;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Pages;

public partial class Maze
{
    private const int MinSize = 1;
    private const int MaxSize = 500;

    private bool _isExitFound;
    private bool _isAtaka;
    private bool _isInit;

    private int _originalSize;
    private int _density;

    private int _boxSize;
    private int _wallWidth;

    private MazeWalls? _mazeWalls;
    private MazeEntities? _mazeSands;
    private MazeRenderParameter? _renderParameter;

    private Labyrinth _labyrinth = null!;
    private MazeSeed _seeder = null!;
    private Vision _vision = null!;

    [Parameter]
    public string? Seed { get; set; }

    [SupplyParameterFromQuery(Name = MazeSeed.SizeQueryName)]
    public int? MazeSize { get; set; }

    [SupplyParameterFromQuery(Name = MazeSeed.DensityQueryName)]
    public int? MazeDensity { get; set; }

    // Проверка на null и инициализацию (дополнительная проверка, если флаг выставили в true, а значение у не null полей не выставили)
    private bool IsInit => _isInit && _labyrinth != null && _seeder != null && _vision != null && _renderParameter != null;

    protected override void OnInitialized()
    {
        _boxSize = 48;
        _wallWidth = Math.Max(1, _boxSize / 10);
    }

    protected override void OnParametersSet()
    {
        _originalSize = MazeSize ?? 16;
        _density = MazeDensity ?? 16;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GenerateAsync();
        }
    }

    private async Task OnMoveKeyDown(MoveEventArgs moveEventArgs)
    {
        if (_isExitFound)
        {
            return;
        }

        if (_isAtaka)
        {
            await Attack(moveEventArgs.Direction);
            _isAtaka = false;
        }

        await Move(moveEventArgs.Direction);
    }

    private async Task OnAttackKeyDown(AttackEventArgs attackEventArgs)
    {
        // TODO Вариант для проверки работоспособности
        switch (attackEventArgs.Type)
        {
            case AttackType.Bomba:
            {
                if (_labyrinth.Items.First(x => x is Bomb).TryUse())
                {
                    await DetonateBomb();
                    _isAtaka = false;
                }

                break;
            }

            case AttackType.Molot:
            {
                if (_labyrinth.Items.First(x => x is Hammer).TryUse())
                {
                    _isAtaka = true;
                }

                break;
            }
        }
    }

    private async Task Attack(Direction direction)
    {
        BreakWall(direction);

        await SoundService.PlayAsync(SoundType.Molot);
        await ForceRenderWalls();
    }

    private async Task DetonateBomb()
    {
        BreakWall(Direction.Left);
        BreakWall(Direction.Top);
        BreakWall(Direction.Right);
        BreakWall(Direction.Bottom);

        await SoundService.PlayAsync(SoundType.Bomb);
        await ForceRenderWalls();
    }

    private void BreakWall(Direction direction)
    {
        _labyrinth.BreakWall(direction);
    }

    private async Task Move(Direction direction)
    {
        _labyrinth.Move(direction);
        _vision.SetPosition(_labyrinth.Player);

        await SoundService.PlayAsync(SoundType.Step);

        Tile tile = _labyrinth[_labyrinth.Player];

        if (tile.IsExit)
        {
            _isExitFound = true;
        }
        else
        {
            if (tile.ItemType != null)
            {
                // TODO вынести в ячейку
                if (tile.ItemType.TryPeekUp())
                {
                    tile.ItemType = null;
                    await SoundService.PlayAsync(SoundType.Score);
                }
            }
        }

        await Task.WhenAll(ForceRenderWalls(), ForceRenderSands());
    }

    private async Task GenerateAsync()
    {
        // todo Костыль чтоб цвет обновлялся, надо больше времени подумать.
        // (Не перерисовывает если стена осталась на прежнем месте)
        // Но в принципе то работает))))))
        StateHasChanged();
        await Task.Delay(1);

        _seeder.Reload();

        _isExitFound = false;

        _originalSize = Math.Max(MinSize, Math.Min(MaxSize, _originalSize));

        _labyrinth = new Labyrinth(_seeder);
        _labyrinth.Init(_originalSize, _originalSize, _density);

        _vision = new Vision(_originalSize, _originalSize);
        _vision.SetPosition(_labyrinth.Player);

        _renderParameter = new MazeRenderParameter(_labyrinth, _boxSize, _wallWidth, _vision);

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
