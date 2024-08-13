using LabirintBlazorApp.Common.Control.Schemes;
using LabirintBlazorApp.Services.Base;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class ControlSchemeSwitcher
{
    [Inject]
    public required IControlSchemeService ControlSchemeService { get; set; }

    private void SwitchScheme(IControlScheme scheme)
    {
        ControlSchemeService.CurrentScheme = scheme;
    }
}
