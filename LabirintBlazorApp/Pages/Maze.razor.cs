using System.Security.Cryptography;
using System.Text;
using LabirintBlazorApp.Common;
using LabirintBlazorApp.Components;
using LabirintBlazorApp.Constants;
using LabirintBlazorApp.Dto;

namespace LabirintBlazorApp.Pages;

public partial class Maze
{
    private const int MinSize = 1;
    private const int MaxSize = 100;

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
    private int _originalSizeDisplay;
    private int _sandCost;
    private int _score;
    private int _speed;

    private int n;
    private int[,] _sand;
    private int[,] lab;
    private int[,] lab2;

    private KeyInterceptor? _keyInterceptor;
    private MazeSands? _mazeSands;
    private MazeWalls? _mazeWalls;

    private Position _exitBox;
    private Position _player;
    private Position _playerDisplay;
    private Random _random = new();

    private road? _way;

    private string? _seed;

    private Vision? _vision;

    protected override void OnInitialized()
    {
        _originalSize = 16;
        _speed = 3;
        _density = 3;
        _sandCost = 100;
        _player = (0, 0);
        _playerDisplay = (0, 0);
        _exitBox = (0, 0);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FocusFieldAsync();
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

        lab[y, x] = 1;
    }

    private async Task Move(int xOffset, int yOffset)
    {
        if (lab[_player.Y + yOffset, _player.X + xOffset] == 0)
        {
            return;
        }

        _player.X += xOffset * 2;
        _player.Y += yOffset * 2;
        _vision?.SetPosition(_player.X, _player.Y);
        _playerDisplay = _player;

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
        _labSize = 0;
        StateHasChanged();
        await Task.Delay(1);

        _random = string.IsNullOrWhiteSpace(_seed) == false
            ? new Random(GetSeed(_seed))
            : new Random();

        _player = (1, 1);
        _playerDisplay = _player;

        _exitNotFound = true;

        _originalSize = ClampSize(_originalSize);
        _originalSizeDisplay = _originalSize;

        n = _originalSize * 2 + 1;

        int mazeWidth = n;
        int mazeHeight = n;
        lab = new int[n, n];

        _labSize = n;

        _score = 0;
        _maxScore = 0;

        _molotCount = MaxMolotCount;
        _bombaCount = MaxBombaCount;

        _vision = new Vision(mazeWidth, mazeHeight);
        _vision.SetPosition(_player.X, _player.Y);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == 0 || i == n - 1)
                {
                    lab[i, j] = 0;
                }

                if (j == 0 || j == n - 1)
                {
                    lab[i, j] = 0;
                }

                if (i % 2 == 1 && j % 2 == 1)
                {
                    lab[i, j] = 2;
                }

                if ((i > 0 && i % 2 == 0 && i < n - 1 && j > 0 && j % 2 == 1 && j < n - 1) || 
                    (i > 0 && i % 2 == 1 && i < n - 1 && j > 0 && j % 2 == 0 && j < n - 1))
                {
                    lab[i, j] = _random.Next(0, _density);
                }
            }

            _exitBox = (n / 2 - (_originalSize % 2 == 0 ? 1 : 0), n);
            lab[_exitBox.Y - 1, _exitBox.X] = 1;
        }

        lab2 = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                lab2[i, j] = 1;
            }
        }

        _sand = lab2;

        _sand[3, 3] = 0;
        _sand[5, 5] = 0;

        for (int i = 1; i < _labSize; i++)
        {
            int x = _random.Next(1, n / 2 + 1) * 2 - 1;
            int y = _random.Next(1, n / 2 + 1) * 2 - 1;

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

        await FocusFieldAsync();
    }

    private Task ForceRenderSands()
    {
        return _mazeSands?.ForceRender() ?? Task.CompletedTask;
    }

    private Task ForceRenderWalls()
    {
        return _mazeWalls?.ForceRender() ?? Task.CompletedTask;
    }

    private static int GetSeed(string input)
    {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        int result = BitConverter.ToInt32(hashBytes, 0);
        return Math.Abs(result);
    }

    private async Task FocusFieldAsync()
    {
        if (_keyInterceptor != null)
        {
            await _keyInterceptor.FocusAsync();
        }
    }

    private int ClampSize(int size)
    {
        size = size switch
        {
            < MinSize => MinSize,
            > MaxSize => MaxSize,
            var _ => size
        };

        return size;
    }

    private async Task FindExitAsync()
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                lab2[i, j] = 1;
            }
        }

        (int y, int x) = _player;

        int back;
        road way = new();
        _exitNotFound = true;

        while (_exitNotFound)
        {
            if (x == n && y == n / 2 || x == n && y == n / 2 - 1)
            {
                _exitNotFound = false;
                _way = way.head;
            }
            else
            {
                StateHasChanged();

                if (_speed is > 0 and < 100)
                {
                    await Task.Delay(100 / _speed);
                }

                if (lab[x + 1, y] != 0 && lab2[x + 1, y] != 0) //down
                {
                    lab2[x + 1, y] = 0;
                    way.add(3);
                    x += 2;

                    _playerDisplay = (y, x);
                }
                else if (lab[x, y + 1] != 0 && lab2[x, y + 1] != 0) //right
                {
                    lab2[x, y + 1] = 0;
                    way.add(1);
                    y += 2;

                    _playerDisplay = (y, x);
                }
                else if (lab[x, y - 1] != 0 && lab2[x, y - 1] != 0) //left
                {
                    lab2[x, y - 1] = 0;
                    way.add(2);
                    y -= 2;

                    _playerDisplay = (y, x);
                }
                else if (lab[x - 1, y] != 0 && lab2[x - 1, y] != 0) //up
                {
                    lab2[x - 1, y] = 0;
                    way.add(4);
                    x -= 2;

                    _playerDisplay = (y, x);
                } //1-r 2-l 3-d 4-u
                else
                {
                    if (way.size() != 0)
                    {
                        back = way.deq();

                        if (back == 1)
                        {
                            y -= 2;
                        }

                        if (back == 2)
                        {
                            y += 2;
                        }

                        if (back == 3)
                        {
                            x -= 2;
                        }

                        if (back == 4)
                        {
                            x += 2;
                        }
                    }
                    else
                    {
                        _exitNotFound = false;
                        _way = null;
                    }
                }
            }
        }
    }
}
