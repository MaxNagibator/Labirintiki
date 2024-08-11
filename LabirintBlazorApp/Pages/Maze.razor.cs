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

    private const int MaxMolotCount = 6;
    private const int MaxBombaCount = 2;
    private bool _exitNotFound;

    private bool _isAtaka;
    private bool _isInit;
    private int _bombaCount;
    private int _density;

    private int _maxScore;

    private int _molotCount;

    private int _originalSize;
    private int _sandCost;
    private int _score;

    private MazeSands? _mazeSands;
    private MazeSeed _seeder = null!;
    private MazeWalls? _mazeWalls;

    private Labyrinth _labyrinth = null!;
    private Vision _vision = null!;

    [Parameter]
    public string? Seed { get; set; }

    protected override void OnInitialized()
    {
        _originalSize = 16;
        _density = 40;
        _sandCost = 100;
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
            case AttackType.Bomba when _bombaCount > 0:
                await DetonateBomb();
                _bombaCount--;
                _isAtaka = false;
                return;

            case AttackType.Molot when _molotCount > 0:
                _isAtaka = true;
                _molotCount--;
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

        if (!_labyrinth.Tiles[_labyrinth.Player.X, _labyrinth.Player.Y].IsExit)
        {
            if (_labyrinth.Tiles[_labyrinth.Player.X, _labyrinth.Player.Y].HasSand)
            {
                _labyrinth.Tiles[_labyrinth.Player.X, _labyrinth.Player.Y].HasSand = false;
                _score += _sandCost;

                await SoundService.PlayAsync(SoundType.Score);
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

        _molotCount = MaxMolotCount;
        _bombaCount = MaxBombaCount;

        _exitNotFound = true;

        _originalSize = Math.Max(MinSize, Math.Min(MaxSize, _originalSize));
        _labyrinth = new Labyrinth();
        _labyrinth.Init(_originalSize, _originalSize, _density, _seeder);
        _maxScore = _sandCost * _labyrinth.SandCount;

        _vision = new Vision(_originalSize, _originalSize);
        _vision.SetPosition(_labyrinth.Player);

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
