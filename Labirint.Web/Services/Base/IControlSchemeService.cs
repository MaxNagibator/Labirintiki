using Labirint.Web.Common.Control.Schemes;

namespace Labirint.Web.Services.Base;

public interface IControlSchemeService
{
    event EventHandler<IControlScheme> ControlSchemeChanged;
    IControlScheme CurrentScheme { get; set; }
    IEnumerable<IControlScheme> AvailableSchemes { get; }
    void RegisterScheme(IControlScheme scheme);
    void UnregisterScheme(IControlScheme scheme);
    void Reset();
}
