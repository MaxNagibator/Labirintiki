using Labirint.Web.Common.Control.Schemes;
using Microsoft.Extensions.Logging;

namespace Labirint.Web.Services;

public class ControlSchemeService
{
    private const string LocalStorageKey = nameof(ControlSchemeService);

    private readonly IReadOnlyList<IControlScheme> _controlSchemes;
    private readonly LocalStorageService _localStorage;
    private readonly ILogger<ControlSchemeService> _logger;
    private IControlScheme _currentScheme;
    private bool _isChosenByUser;

    public ControlSchemeService(LocalStorageService localStorage, ILogger<ControlSchemeService> logger)
    {
        _localStorage = localStorage;
        _logger = logger;

        _controlSchemes =
        [
            new ClassicScheme(),
            new AlternativeScheme(),
        ];

        _currentScheme = _controlSchemes[0];
        _ = LoadCurrentSchemeAsync();
    }

    public event EventHandler<IControlScheme>? ControlSchemeChanged;

    public IControlScheme CurrentScheme => _currentScheme;

    public IEnumerable<IControlScheme> AvailableSchemes => _controlSchemes;

    public IControlScheme DefaultScheme => _controlSchemes[0];

    public async Task SetSchemeAsync(IControlScheme scheme)
    {
        if (_controlSchemes.Contains(scheme) == false)
        {
            throw new ArgumentException($"Предоставленная схема управления «{scheme.Name}» не зарегистрирована.", nameof(scheme));
        }

        var wasChosenByUser = _isChosenByUser;
        _isChosenByUser = true;

        try
        {
            await _localStorage.SetItemAsync(LocalStorageKey, scheme.Name);
        }
        catch
        {
            _isChosenByUser = wasChosenByUser;
            throw;
        }

        _currentScheme = scheme;
        NotifySchemeChanged();
    }

    private void NotifySchemeChanged()
    {
        ControlSchemeChanged?.Invoke(this, _currentScheme);
    }

    private async Task LoadCurrentSchemeAsync()
    {
        string? schemeName;

        try
        {
            schemeName = await _localStorage.GetItemAsync<string>(LocalStorageKey);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Не удалось прочитать схему управления, остаётся схема по умолчанию");
            return;
        }

        if (schemeName == null || _isChosenByUser)
        {
            return;
        }

        var scheme = _controlSchemes.FirstOrDefault(controlScheme => controlScheme.Name == schemeName);

        if (scheme == null)
        {
            return;
        }

        _currentScheme = scheme;
        NotifySchemeChanged();
    }
}
