using LabirintBlazorApp.Common.Control.Schemes;

namespace LabirintBlazorApp.Services;

public class ControlSchemeService : IControlSchemeService
{
    private readonly List<IControlScheme> _controlSchemes;
    private IControlScheme _currentScheme;

    public ControlSchemeService()
    {
        _controlSchemes =
        [
            new ClassicScheme(),
            new AlternativeScheme()
        ];

        _currentScheme = _controlSchemes.First();
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
        NotifySchemeChanged();
    }

    private void NotifySchemeChanged()
    {
        ControlSchemeChanged?.Invoke(this, _currentScheme);
    }
}
