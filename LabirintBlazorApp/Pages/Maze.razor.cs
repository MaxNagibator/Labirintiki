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

    private int _exitBoxPositionX;
    private int _exitBoxPositionY;
    private int _labSize;

    private int _maxScore;
    private int _molotCount;

    private int _myPositionX;
    private int _myPositionXDisplay;

    private int _myPositionY;
    private int _myPositionYDisplay;

    private int _originalSize;
    private int _originalSizeDisplay;

    private int _sandCost;
    private int _score;
    private Vision _vision;
    private int _speed;

    private int n;
    private int[,] _sand;
    private int[,] lab;
    private int[,] lab2;

    private KeyInterceptor? _keyInterceptor;
    private MazeSands? _mazeSands;
    private MazeWalls? _mazeWalls;

    private Random _random = new();
    private road? _way;

    private string? _seed;

    protected override void OnInitialized()
    {
        _originalSize = 16;
        _speed = 3;
        _density = 3;
        _sandCost = 100;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FocusFieldAsync();
            await GenerateAsync();
            StateHasChanged();
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
            Attack(moveEventArgs.DeltaX, moveEventArgs.DeltaY);
            await SoundService.PlayAsync(SoundType.Molot);
            _isAtaka = false;
        }

        await Move(moveEventArgs.DeltaX, moveEventArgs.DeltaY);
        StateHasChanged();
    }

    private void Attack(int deltaX, int deltaY)
    {
        BreakWall(_myPositionX + deltaX, _myPositionY + deltaY);
    }

    private async void OnAttackKeyDown(AttackEventArgs attackEventArgs)
    {
        switch (attackEventArgs.Type)
        {
            case AttackType.Bomba when _bombaCount > 0:
                DetonateBomb();
                _bombaCount--;
                await SoundService.PlayAsync(SoundType.Bomb);
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

    private void DetonateBomb()
    {
        BreakWall(_myPositionX + 1, _myPositionY);
        BreakWall(_myPositionX - 1, _myPositionY);
        BreakWall(_myPositionX, _myPositionY - 1);
        BreakWall(_myPositionX, _myPositionY + 1);
    }

    private void BreakWall(int x, int y)
    {
        if (x <= 0 || x >= _originalSize * 2 || y <= 0 || y >= _originalSize * 2)
        {
            return;
        }

        lab[y, x] = 1;
        _mazeWalls?.UpdateAsync([(x, y)]);
    }

    private async Task Move(int xOffset, int yOffset)
    {
        if (lab[_myPositionY + yOffset, _myPositionX + xOffset] == 0)
        {
            return;
        }

        _myPositionX += xOffset * 2;
        _myPositionY += yOffset * 2;
        _vision.SetPosition(_myPositionX, _myPositionY);
        _myPositionXDisplay = _myPositionX;
        _myPositionYDisplay = _myPositionY;

        if (_mazeWalls != null)
        {
            await _mazeWalls.ForceRender();
        }

        if (_mazeSands != null)
        {
            await _mazeSands.ForceRender();
        }

        await SoundService.PlayAsync(SoundType.Step);

        if (_myPositionX == _exitBoxPositionX && _myPositionY == _exitBoxPositionY)
        {
            _exitNotFound = false;
            return;
        }

        if (_sand[_myPositionY, _myPositionX] != 0)
        {
            return;
        }

        _sand[_myPositionY, _myPositionX] = 1;
        _mazeSands?.UpdateAsync(_myPositionX, _myPositionY);
        _score += _sandCost;

        if (_mazeSands != null)
        {
            await _mazeSands.ForceRender();
        }

        await SoundService.PlayAsync(SoundType.Score);
        StateHasChanged();
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

        _myPositionX = 1;
        _myPositionY = 1;
        _myPositionXDisplay = _myPositionX;
        _myPositionYDisplay = _myPositionY;
        _exitNotFound = true;
        _originalSize = ClampSize(_originalSize);
        _originalSizeDisplay = _originalSize;
        n = _originalSize * 2 + 1;
        var mazeWidth = n;
        var mazeHeight = n;
        lab = new int[n, n];
        _labSize = n;
        _maxScore = 0;
        _molotCount = MaxMolotCount;
        _bombaCount = MaxBombaCount;
        _score = 0;


        _vision = new Vision(mazeWidth, mazeHeight);
        _vision.SetPosition(_myPositionX, _myPositionY);
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

            _exitBoxPositionX = n / 2 - (_originalSize % 2 == 0 ? 1 : 0);
            _exitBoxPositionY = n;
            lab[_exitBoxPositionY - 1, _exitBoxPositionX] = 1;
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

            if (_sand[x, y] == 0)
            {
                continue;
            }

            _sand[x, y] = 0;
            _maxScore++;
        }

        _maxScore *= _sandCost;

        StateHasChanged();

        if (_mazeWalls != null)
        {
            await _mazeWalls.ForceRender();
        }

        if (_mazeSands != null)
        {
            await _mazeSands.ForceRender();
        }

        _isInit = true;
        StateHasChanged();

        await FocusFieldAsync();
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

        int x = _myPositionY;
        int y = _myPositionX;
        int back;
        road way = new();
        _exitNotFound = true;

        while (_exitNotFound)
        {
            if (x == n && y == n / 2 || x == n && y == n / 2 - 1)
            {
                //textresult.Text = "exit: true";
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

                    _myPositionXDisplay = y;
                    _myPositionYDisplay = x;
                }
                else if (lab[x, y + 1] != 0 && lab2[x, y + 1] != 0) //right
                {
                    lab2[x, y + 1] = 0;
                    way.add(1);
                    y += 2;

                    _myPositionXDisplay = y;
                    _myPositionYDisplay = x;
                }
                else if (lab[x, y - 1] != 0 && lab2[x, y - 1] != 0) //left
                {
                    lab2[x, y - 1] = 0;
                    way.add(2);
                    y -= 2;

                    _myPositionXDisplay = y;
                    _myPositionYDisplay = x;
                }
                else if (lab[x - 1, y] != 0 && lab2[x - 1, y] != 0) //up
                {
                    lab2[x - 1, y] = 0;
                    way.add(4);
                    x -= 2;

                    _myPositionXDisplay = y;
                    _myPositionYDisplay = x;
                } //1-r 2-l 3-d 4-u
                else
                {
                    if (way.size() != 0)
                    {
                        back = way.deq();

                        if (back == 1) { y -= 2; }

                        if (back == 2) { y += 2; }

                        if (back == 3) { x -= 2; }

                        if (back == 4) { x += 2; }
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
