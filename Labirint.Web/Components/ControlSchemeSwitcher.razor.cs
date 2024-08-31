using Labirint.Web.Common.Control.Schemes;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components;

public partial class ControlSchemeSwitcher : IDisposable
{
    [Inject]
    public required IControlSchemeService ControlSchemeService { get; set; }

    public void Dispose()
    {
        ControlSchemeService.ControlSchemeChanged -= OnControlSchemeChanged;

        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        ControlSchemeService.ControlSchemeChanged += OnControlSchemeChanged;
    }

    private void OnControlSchemeChanged(object? sender, IControlScheme e)
    {
        StateHasChanged();
    }

    private void SwitchScheme(IControlScheme scheme)
    {
        ControlSchemeService.CurrentScheme = scheme;
    }
}
