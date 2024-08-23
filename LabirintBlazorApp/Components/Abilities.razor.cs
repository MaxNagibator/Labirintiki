using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class Abilities
{
    [Parameter]
    public required IReadOnlyList<RunnerAbility> RunnerAbilities { get; set; }
}
