using Labirint.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Pages;

public partial class Home
{
    private const int HeroSeed = 20250813;

    private readonly IReadOnlyList<string> _facts =
    [
        "Первый известный лабиринт был построен в Египте около 4 тысяч лет назад. Он был частью комплекса пирамид в Хаваре.",
        "Легендарный лабиринт Минотавра, который, как говорят, находился на Крите, был спроектирован архитектором Дедалом.",
        "Лабиринты использовались в древних культурах для различных религиозных и ритуальных целей, таких как медитация и посвящение.",
        "В Европе в эпоху Средневековья многие соборы и церкви имели лабиринты на полу, которые паломники проходили на коленях.",
        "Существует множество видов лабиринтов, включая классические круговые, трехмерные и даже живые лабиринты, созданные из растений.",
        "Современные лабиринты часто создаются для развлечений, таких как парки развлечений, или в терапевтических целях, чтобы помочь людям расслабиться.",
    ];

    private readonly IReadOnlyList<string> _jokes =
    [
        "Почему лабиринты всегда такие спокойные? Потому что они знают, что все пути ведут к выходу!",
        "Как называется человек, который постоянно застревает в лабиринте? Лабиринтоход!",
        "Пойдем по кружке кофе, но не заблудись по пути!",
        "Почему программисты не боятся лабиринтов? Потому что они всегда найдут выход с помощью алгоритма!",
        "Мальчик сломал руку в шести местах когда показывал как пройти лабиринт!",
    ];

    [SupplyParameterFromQuery(Name = RandomGenerator.SeedQueryName)]
    public string? Seed { get; set; }

    [SupplyParameterFromQuery(Name = RandomGenerator.SizeQueryName)]
    public string? MazeSize { get; set; }

    [SupplyParameterFromQuery(Name = RandomGenerator.DensityQueryName)]
    public string? MazeDensity { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    private bool IsSharedGame => string.IsNullOrWhiteSpace(Seed) == false;

    protected override void OnParametersSet()
    {
        if (IsSharedGame == false)
        {
            return;
        }

        var link = RandomGenerator.BuildRouteLink(NavigationManager,
            Seed!,
            RandomGenerator.ParseQueryNumber(MazeSize),
            RandomGenerator.ParseQueryNumber(MazeDensity));

        NavigationManager.NavigateTo(link, replace: true);
    }

    private void StartGame()
    {
        NavigationManager.NavigateTo("labirint");
    }
}
