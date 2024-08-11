using LabirintBlazorApp.Common.Schemes;

namespace LabirintBlazorApp.Components;

public partial class ControlSchemeSwitcher
{
    private void SwitchScheme(IControlScheme scheme)
    {
        ControlSchemeService.CurrentScheme = scheme;
    }
}
