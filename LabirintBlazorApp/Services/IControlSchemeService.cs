using LabirintBlazorApp.Common.Schemes;

namespace LabirintBlazorApp.Services;

public interface IControlSchemeService
{
    IControlScheme CurrentScheme { get; set; }
    IEnumerable<IControlScheme> AvailableSchemes { get; }
    event EventHandler<IControlScheme> ControlSchemeChanged;
    void RegisterScheme(IControlScheme scheme);
    void UnregisterScheme(IControlScheme scheme);
}
