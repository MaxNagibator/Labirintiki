using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class RunnerAbilities
{
    [Parameter]
    public required IReadOnlyList<RunnerAbility> Abilities { get; set; }
}
