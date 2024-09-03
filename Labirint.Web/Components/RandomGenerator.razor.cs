using System.Security.Cryptography;
using System.Text;
using Labirint.Core.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Labirint.Web.Components;

public partial class RandomGenerator : IRandom
{
    public const string SizeQueryName = "s";
    public const string DensityQueryName = "d";
    private const string MazePageUrl = "labirint";

    private bool _isShowMotivation;
    private int _currentSeed;

    private MudMessageBox? _messageBox;
    private Random? _random;
    private string? _userSeed;

    public Random Generator => _random ?? Random.Shared;

    public string Link => GetShareLink();

    [Parameter]
    public string? Seed { get; set; }

    [Parameter]
    public int Size { get; set; }

    [Parameter]
    public int Density { get; set; }

    [Parameter]
    public bool IsExitFound { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private IClipboardService ClipboardService { get; set; } = null!;

    [Inject]
    private ISnackbar SnackbarService { get; set; } = null!;

    private bool IsGenerateRequired => _currentSeed < 0;

    public void Reload(bool force = false)
    {
        if (force || string.IsNullOrWhiteSpace(_userSeed))
        {
            ReloadWithRandomSeed();
            return;
        }

        _currentSeed = _userSeed.All(char.IsDigit)
            ? int.Parse(_userSeed)
            : GenerateSeed(_userSeed);

        _random = new Random(_currentSeed);
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(Seed) == false)
        {
            _userSeed = Seed;
        }
    }

    private static int GenerateSeed(string input)
    {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        int result = BitConverter.ToInt32(hashBytes, 0);
        return Math.Abs(result);
    }

    private void ReloadWithRandomSeed()
    {
        _userSeed = null;
        _currentSeed = Random.Shared.Next();
        _random = new Random(_currentSeed);
        StateHasChanged();
    }

    private void ResetSeed()
    {
        _currentSeed = -1;
        StateHasChanged();
    }

    private string GetShareLink()
    {
        string linkWithSeed = $"{NavigationManager.BaseUri}{MazePageUrl}/{_currentSeed}";

        return NavigationManager.GetUriWithQueryParameters(linkWithSeed, new Dictionary<string, object?>
        {
            [SizeQueryName] = Size,
            [DensityQueryName] = Density
        });
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

    private void ToggleMotivation()
    {
        _isShowMotivation = !_isShowMotivation;

        _messageBox?.Close();
        _messageBox?.ShowAsync();
    }
}
