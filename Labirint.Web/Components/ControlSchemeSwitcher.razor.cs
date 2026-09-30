using Labirint.Web.Common.Control.Schemes;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components;

public partial class ControlSchemeSwitcher
{
    private readonly string _labelId = $"scheme-{Guid.NewGuid():N}";

    [Inject]
    public required ControlSchemeService ControlSchemeService { get; set; }

    [Parameter]
    [EditorRequired]
    public required IControlScheme Value { get; set; }

    [Parameter]
    public EventCallback<IControlScheme> ValueChanged { get; set; }

    private Task SwitchSchemeAsync(IControlScheme scheme)
    {
        return ValueChanged.InvokeAsync(scheme);
    }
}
