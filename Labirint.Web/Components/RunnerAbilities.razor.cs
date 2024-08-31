using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components;

public partial class RunnerAbilities
{
    [Parameter]
    public required IReadOnlyList<RunnerAbility> Abilities { get; set; }
}
