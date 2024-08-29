using Blazored.LocalStorage;
using LabirintBlazorApp.Common.Control.Schemes;

namespace LabirintBlazorApp.Services;

public class ControlSchemeService : IControlSchemeService
{
    private const string CurrentSchemeKey = "CurrentScheme";

    private readonly List<IControlScheme> _controlSchemes;
    private readonly ILocalStorageService _localStorage;
    private IControlScheme _currentScheme;

    public ControlSchemeService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;

        _controlSchemes =
        [
            new ClassicScheme(),
            new AlternativeScheme()
        ];

        _currentScheme = _controlSchemes.First();
        LoadCurrentSchemeAsync().ConfigureAwait(false);
    }

    public event EventHandler<IControlScheme>? ControlSchemeChanged;

    public IControlScheme CurrentScheme
    {
        get => _currentScheme;
        set
        {
            if (_controlSchemes.Contains(value))
            {
                _currentScheme = value;
                SaveCurrentSchemeAsync().ConfigureAwait(false);
                NotifySchemeChanged();
            }
            else
            {
                throw new ArgumentException($"Предоставленная схема управления «{value.GetType().Name}» не зарегистрирована.");
            }
        }
    }

    public IEnumerable<IControlScheme> AvailableSchemes => _controlSchemes;

    public void RegisterScheme(IControlScheme scheme)
    {
        if (_controlSchemes.Contains(scheme))
        {
            return;
        }

        _controlSchemes.Add(scheme);
    }

    public void UnregisterScheme(IControlScheme scheme)
    {
        _controlSchemes.Remove(scheme);

        if (_currentScheme != scheme)
        {
            return;
        }

        _currentScheme = _controlSchemes.FirstOrDefault() ?? new ClassicScheme();
        SaveCurrentSchemeAsync().ConfigureAwait(false);
        NotifySchemeChanged();
    }

    private void NotifySchemeChanged()
    {
        ControlSchemeChanged?.Invoke(this, _currentScheme);
    }

    private async Task LoadCurrentSchemeAsync()
    {
        string? schemeName = await _localStorage.GetItemAsync<string>(CurrentSchemeKey);

        if (schemeName != null)
        {
            IControlScheme? scheme = _controlSchemes.FirstOrDefault(controlScheme => controlScheme.Name == schemeName);

            if (scheme != null)
            {
                _currentScheme = scheme;
                NotifySchemeChanged();
            }
        }
    }

    private async Task SaveCurrentSchemeAsync()
    {
        await _localStorage.SetItemAsync(CurrentSchemeKey, _currentScheme.Name);
    }
}
