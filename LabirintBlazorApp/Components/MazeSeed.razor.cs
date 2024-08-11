using System.Security.Cryptography;
using System.Text;
using LabirintBlazorApp.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LabirintBlazorApp.Components;

public partial class MazeSeed
{
    private int _newSeed;
    private int _seed;

    private MudMessageBox? _messageBox;

    private Random? _random;
    private string? _userSeed;

    private string Link => $"{NavigationManager.BaseUri}labirint/{_seed}";

    [Parameter]
    public string? Seed { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private IClipboardService ClipboardService { get; set; } = null!;

    [Inject]
    private ISnackbar SnackbarService { get; set; } = null!;

    public Random Random => _random ?? Random.Shared;

    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(Seed))
        {
            _seed = Random.Shared.Next();
            _random = new Random(_seed);
        }
        else
        {
            _userSeed = Seed;
            Reset();
        }
    }

    private void Reset()
    {
        if (string.IsNullOrWhiteSpace(_userSeed))
        {
            return;
        }

        _newSeed = _userSeed.All(char.IsDigit)
            ? int.Parse(_userSeed)
            : GenerateSeed(_userSeed);

        if (_seed == _newSeed)
        {
            return;
        }

        _random = new Random(_newSeed);
        _seed = _newSeed;
    }

    private static int GenerateSeed(string input)
    {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        int result = BitConverter.ToInt32(hashBytes, 0);
        return Math.Abs(result);
    }

    private void ShowMessageBox()
    {
        _messageBox?.ShowAsync();
    }

    private async Task CopyLink()
    {
        await ClipboardService.CopyToClipboard(Link);
        SnackbarService.Add("Ссылка скопирована!", Severity.Success);
    }
}
