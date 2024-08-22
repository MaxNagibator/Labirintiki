using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class Abilities
{
    [Parameter]
    public required List<RunnerAbility> RunnerAbilities { get; set; }
}
