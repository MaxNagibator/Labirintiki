using LabirintBlazorApp.Common.Control.Schemes;

namespace LabirintBlazorApp.Services.Base;

public interface IControlSchemeService
{
    event EventHandler<IControlScheme> ControlSchemeChanged;
    IControlScheme CurrentScheme { get; set; }
    IEnumerable<IControlScheme> AvailableSchemes { get; }
    void RegisterScheme(IControlScheme scheme);
    void UnregisterScheme(IControlScheme scheme);
}
