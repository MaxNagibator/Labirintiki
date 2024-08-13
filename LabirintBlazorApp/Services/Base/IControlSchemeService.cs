using LabirintBlazorApp.Common.Control.Schemes;

namespace LabirintBlazorApp.Services.Base;

public interface IControlSchemeService
{
    IControlScheme CurrentScheme { get; set; }
    IEnumerable<IControlScheme> AvailableSchemes { get; }
    event EventHandler<IControlScheme> ControlSchemeChanged;
    void RegisterScheme(IControlScheme scheme);
    void UnregisterScheme(IControlScheme scheme);
}
