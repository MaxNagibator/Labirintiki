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

    private int _labSize;

    private int _maxScore;

    private int _molotCount;

    private int _originalSize;
    private int _sandCost;
    private int _score;
    private int[,] _lab;
    private int[,] _sand;

    private MazeSands? _mazeSands;
    private MazeSeed _seeder = null!;
    private MazeWalls? _mazeWalls;

    private Position _exitBox;
    private Position _player;

    private Vision _vision = null!;

    [Parameter]
    public string? Seed { get; set; }

    protected override void OnInitialized()
    {
        _originalSize = 16;
        _density = 3;
        _sandCost = 100;
        _player = (0, 0);
        _exitBox = (0, 0);
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
            await Attack(moveEventArgs.DeltaX, moveEventArgs.DeltaY);
            _isAtaka = false;
        }

        await Move(moveEventArgs.DeltaX, moveEventArgs.DeltaY);
    }

    private async Task Attack(int deltaX, int deltaY)
    {
        BreakWall(_player.X + deltaX, _player.Y + deltaY);

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
        BreakWall(_player.X + 1, _player.Y);
        BreakWall(_player.X - 1, _player.Y);
        BreakWall(_player.X, _player.Y - 1);
        BreakWall(_player.X, _player.Y + 1);

        await SoundService.PlayAsync(SoundType.Bomb);
        await ForceRenderWalls();
    }

    private void BreakWall(int x, int y)
    {
        if (x <= 0 || x >= _originalSize * 2 || y <= 0 || y >= _originalSize * 2)
        {
            return;
        }

        _lab[y, x] = 1;
    }

    private async Task Move(int xOffset, int yOffset)
    {
        if (_lab[_player.Y + yOffset, _player.X + xOffset] == 0)
        {
            return;
        }

        _player.X += xOffset * 2;
        _player.Y += yOffset * 2;
        _vision.SetPosition(_player);

        await SoundService.PlayAsync(SoundType.Step);

        if (_player != _exitBox)
        {
            if (_sand[_player.Y, _player.X] == 0)
            {
                _sand[_player.Y, _player.X] = 1;
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

        _player = (1, 1);
        _exitNotFound = true;

        _originalSize = Math.Max(MinSize, Math.Min(MaxSize, _originalSize));
        _labSize = _originalSize * 2 + 1;
        _lab = new int[_labSize, _labSize];

        _vision = new Vision(_labSize, _labSize);
        _vision.SetPosition(_player);

        for (int i = 0; i < _labSize; i++)
        {
            for (int j = 0; j < _labSize; j++)
            {
                if (i == 0 || i == _labSize - 1)
                {
                    _lab[i, j] = 0;
                }

                if (j == 0 || j == _labSize - 1)
                {
                    _lab[i, j] = 0;
                }

                if (i % 2 == 1 && j % 2 == 1)
                {
                    _lab[i, j] = 2;
                }

                if ((i > 0 && i % 2 == 0 && i < _labSize - 1 && j > 0 && j % 2 == 1 && j < _labSize - 1) || 
                    (i > 0 && i % 2 == 1 && i < _labSize - 1 && j > 0 && j % 2 == 0 && j < _labSize - 1))
                {
                    _lab[i, j] = _seeder.Random.Next(0, _density);
                }
            }

            _exitBox = (_labSize / 2 - (_originalSize % 2 == 0 ? 1 : 0), _labSize);
            _lab[_exitBox.Y - 1, _exitBox.X] = 1;
        }

        _sand = new int[_labSize, _labSize];

        for (int i = 0; i < _labSize; i++)
        {
            for (int j = 0; j < _labSize; j++)
            {
                _sand[i, j] = 1;
            }
        }

        for (int i = 1; i < _labSize; i++)
        {
            int x = _seeder.Random.Next(1, _labSize / 2 + 1) * 2 - 1;
            int y = _seeder.Random.Next(1, _labSize / 2 + 1) * 2 - 1;

            if (_sand[x, y] != 0)
            {
                _sand[x, y] = 0;
                _maxScore++;
            }
            else
            {
                i--;
            }
        }

        _maxScore *= _sandCost;

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
