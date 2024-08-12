using LabirintBlazorApp.Common;
using LabirintBlazorApp.Components;
using LabirintBlazorApp.Constants;
using LabirintBlazorApp.Dto;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Pages;

public partial class Maze
{
    private const int MinSize = 1;
    private const int MaxSize = 500;

    private const int MaxHammerCount = 6;
    private const int MaxBombCount = 2;
    private const int SandCost = 100;

    private int _hammerCount;
    private int _bombCount;
    private bool _isAtaka;

    private bool _exitNotFound;
    private bool _isInit;

    private int _originalSize = 16;
    private int _density = 40;
    private int _score;
    private int _maxScore;

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

    private bool IsInit => _isInit && _labyrinth != null && _seeder != null && _vision != null && _renderParameter != null;

    protected override void OnInitialized()
    {
        _boxSize = 48;
        _wallWidth = Math.Max(1, _boxSize / 10);
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
        if (_exitNotFound == false)
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

    private async Task Attack(Direction direction)
    {
        BreakWall(direction);

        await SoundService.PlayAsync(SoundType.Molot);
        await ForceRenderWalls();
    }

    private async Task OnAttackKeyDown(AttackEventArgs attackEventArgs)
    {
        switch (attackEventArgs.Type)
        {
            case AttackType.Bomba when _bombCount > 0:
                await DetonateBomb();
                _bombCount--;
                _isAtaka = false;
                return;

            case AttackType.Molot when _hammerCount > 0:
                _isAtaka = true;
                _hammerCount--;
                break;

            case AttackType.None:
            default:
                break;
        }
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

        if (!_labyrinth[_labyrinth.Player].IsExit)
        {
            if (_labyrinth[_labyrinth.Player].ItemType != null)
            {
                if (_labyrinth[_labyrinth.Player].ItemType == ItemType.Sand)
                {
                    _score += SandCost;
                    _labyrinth[_labyrinth.Player].ItemType = null;
                    await SoundService.PlayAsync(SoundType.Score);
                }
                else
                if (_labyrinth[_labyrinth.Player].ItemType == ItemType.Hammer)
                {
                    if (_hammerCount < MaxHammerCount)
                    {
                        _hammerCount++;
                        _labyrinth[_labyrinth.Player].ItemType = null;
                        await SoundService.PlayAsync(SoundType.Score);
                    }
                }
                else
                if (_labyrinth[_labyrinth.Player].ItemType == ItemType.Bomb)
                {
                    if (_bombCount < MaxBombCount)
                    {
                        _bombCount++;
                        _labyrinth[_labyrinth.Player].ItemType = null;
                        await SoundService.PlayAsync(SoundType.Score);
                    }
                }
            }
        }
        else
        {
            _exitNotFound = false;
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

        _score = 0;
        _maxScore = 0;

        _hammerCount = MaxHammerCount;
        _bombCount = MaxBombCount;

        _exitNotFound = true;

        _originalSize = Math.Max(MinSize, Math.Min(MaxSize, _originalSize));

        _labyrinth = new Labyrinth(_seeder);
        _labyrinth.Init(_originalSize, _originalSize, _density);

        _maxScore = SandCost * _labyrinth.SandCount;

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
