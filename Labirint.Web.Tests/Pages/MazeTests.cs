using System.Globalization;
using Bunit;
using Labirint.Core;
using Labirint.Core.Items;
using Labirint.Core.Items.Base;
using Labirint.Web.Common.Animation;
using Labirint.Web.Common.Seeding;
using Labirint.Web.Components;
using Labirint.Web.Components.Dialogs;
using Labirint.Web.Components.Ui;
using Labirint.Web.Pages;
using Labirint.Web.Parameters;
using Labirint.Web.Services;
using Labirint.Web.Services.Dialogs;
using Labirint.Web.Services.Toasts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Labirint.Web.Tests.Pages;

[TestFixture]
public class MazeTests
{
    private const string InitialSeed = "1";
    private const int InitialSize = 16;
    private const int SlowGenerationSize = 500;

    private const string FocusBySelector = "Blazor._internal.domWrapper.focusBySelector";

    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(5);

    private BunitContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new();
        _context.JSInterop.Mode = JSRuntimeMode.Loose;
        _context.Services.AddLogging();
        _context.Services.AddSingleton<LocalStorageService>();
        _context.Services.AddSingleton<LabyrinthParametersService>();
        _context.Services.AddSingleton<SoundService>();
        _context.Services.AddSingleton<AnimationService>();
        _context.Services.AddSingleton<ControlSchemeService>();
        _context.Services.AddScoped<ClipboardService>();
        _context.Services.AddScoped<PickupFlightService>();
        _context.Services.AddScoped<MotionService>();
        _context.Services.AddScoped<DialogService>();
        _context.Services.AddScoped<ToastService>();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Тестирует, что страница Maze закрывает ход и бросок предмета, пока идёт перегенерация лабиринта.
    /// Проверяет, что шаг и активация бомбы, пришедшие между нажатием «Генерировать» и концом генерации, не сдвигают бегуна и не расходуют предмет.
    /// </summary>
    [Test]
    public async Task InputIsIgnoredDuringGenerationTest()
    {
        var rendered = await RenderMazeAsync(InitialSeed, SlowGenerationSize);
        var interceptor = rendered.FindComponent<KeyInterceptor>();
        var inventory = GetLabyrinth(rendered).Runner.Inventory;
        var bomb = GiveBomb(inventory);
        var useCount = CountUses(inventory);

        var generation = rendered.FindAll("button").Single(button => button.TextContent.Contains("Генерировать")).ClickAsync(new());

        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Right));
        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(GetActivateKey(bomb)));

        Assert.That(generation.IsCompleted, Is.False);

        await generation;
        await WaitForCastAsync(rendered);

        Assert.Multiple(() =>
        {
            Assert.That(GetAnnouncements(rendered), Does.Not.Contain("Ряд"));
            Assert.That(GetLabyrinth(rendered).Runner.Position, Is.EqualTo(new Position(0, 0)));
            Assert.That(useCount(), Is.Zero);
        });
    }

    /// <summary>
    /// Тестирует, что свайп по полю идёт через KeyInterceptor и на паузе не становится ходом.
    /// Проверяет, что на паузе бегун остаётся на старте и счётчик ходов равен нулю, а после снятия паузы тот же свайп делает ход.
    /// </summary>
    [Test]
    public async Task PausedSwipeIsIgnoredTest()
    {
        var rendered = await RenderMazeAsync(InitialSeed, InitialSize);
        var field = rendered.FindComponent<MazeField>();
        var interceptor = rendered.FindComponent<KeyInterceptor>();

        await TogglePauseAsync(interceptor);
        await field.InvokeAsync(() => field.Instance.OnSwipe.InvokeAsync(Direction.Right));

        Assert.Multiple(() =>
        {
            Assert.That(GetLabyrinth(rendered).Runner.Position, Is.EqualTo(new Position(0, 0)));
            Assert.That(GetMoveCount(rendered), Is.EqualTo("0"));
        });

        await TogglePauseAsync(interceptor);
        await field.InvokeAsync(() => field.Instance.OnSwipe.InvokeAsync(Direction.Right));

        rendered.WaitForState(() => GetMoveCount(rendered) == "1", WaitTimeout);
        Assert.That(GetLabyrinth(rendered).Runner.Position, Is.EqualTo(new Position(1, 0)));
    }

    /// <summary>
    /// Тестирует, что отложенный бросок предмета сверяет паузу после полёта иконки, а не только в момент нажатия.
    /// Проверяет, что бомба, активированная до паузы, на паузе не расходуется, а без паузы расходуется ровно одна.
    /// </summary>
    /// <param name="isPaused">Ставится ли пауза, пока иконка предмета в полёте</param>
    /// <param name="expectedUses">Ожидаемое число использований бомбы</param>
    [TestCase(true, 0)]
    [TestCase(false, 1)]
    public async Task PauseCancelsDelayedCastTest(bool isPaused, int expectedUses)
    {
        var rendered = await RenderMazeAsync(InitialSeed, InitialSize);
        var interceptor = rendered.FindComponent<KeyInterceptor>();
        var inventory = GetLabyrinth(rendered).Runner.Inventory;
        var bomb = GiveBomb(inventory);
        var bombCount = GetCount(inventory, bomb);
        var useCount = CountUses(inventory);

        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(GetActivateKey(bomb)));

        if (isPaused)
        {
            await TogglePauseAsync(interceptor);
        }

        await WaitForCastAsync(rendered);

        Assert.Multiple(() =>
        {
            Assert.That(useCount(), Is.EqualTo(expectedUses));
            Assert.That(GetCount(inventory, bomb), Is.EqualTo(bombCount - expectedUses));
        });
    }

    /// <summary>
    /// Тестирует, что смена зерна или размера в адресе на открытой странице перегенерирует лабиринт, а повтор того же адреса – нет.
    /// Проверяет, что после смены маршрута бегун возвращается на старт, счётчик ходов обнуляется и размер поля совпадает с адресом, а при прежнем маршруте сделанный ход сохраняется.
    /// </summary>
    /// <param name="seed">Зерно в новом адресе</param>
    /// <param name="size">Размер в новом адресе</param>
    /// <param name="isRegenerated">Ожидается ли перегенерация</param>
    [TestCase("2", InitialSize, true)]
    [TestCase(InitialSeed, 8, true)]
    [TestCase(InitialSeed, InitialSize, false)]
    public async Task RouteChangeRegeneratesMazeTest(string seed, int size, bool isRegenerated)
    {
        var rendered = await RenderMazeAsync(InitialSeed, InitialSize);
        var interceptor = rendered.FindComponent<KeyInterceptor>();

        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Right));
        rendered.WaitForState(() => GetMoveCount(rendered) == "1", WaitTimeout);

        Navigate(seed, size);
        rendered.Render(parameters => parameters.Add(maze => maze.Seed, seed));

        if (isRegenerated)
        {
            rendered.WaitForState(() => IsReady(rendered) && GetLabyrinth(rendered).Runner.Position == new Position(0, 0), WaitTimeout);
        }

        Assert.Multiple(() =>
        {
            Assert.That(IsReady(rendered), Is.True);
            Assert.That(GetMoveCount(rendered), Is.EqualTo(isRegenerated ? "0" : "1"));
            Assert.That(GetLabyrinth(rendered).Runner.Position, Is.EqualTo(isRegenerated ? new Position(0, 0) : new Position(1, 0)));
            Assert.That(GetLabyrinth(rendered).Width, Is.EqualTo(size));
        });
    }

    /// <summary>
    /// Тестирует, что выход на клетку выхода поднимает диалог финала и закрывает ход до решения игрока.
    /// Проверяет, что открывается ровно один WinDialog с заголовком «Финал Лабиринта», ход после финала игнорируется, а после «Продолжить скитание» снова засчитывается.
    /// </summary>
    [Test]
    public async Task ExitShowsWinDialogTest()
    {
        var rendered = await RenderMazeAsync(InitialSeed, 2, 0);
        var interceptor = rendered.FindComponent<KeyInterceptor>();
        var dialogs = _context.Services.GetRequiredService<DialogService>();

        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Right));
        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Bottom));

        rendered.WaitForState(() => dialogs.Instances.Count == 1, WaitTimeout);

        var dialog = dialogs.Instances.Single();

        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Top));

        Assert.Multiple(() =>
        {
            Assert.That(dialog.ContentType, Is.EqualTo(typeof(WinDialog)));
            Assert.That(dialog.Title, Is.EqualTo("Финал Лабиринта"));
            Assert.That(GetLabyrinth(rendered).Runner.Position, Is.EqualTo(new Position(1, 1)));
        });

        await rendered.InvokeAsync(() => dialog.Close(true));
        await rendered.InvokeAsync(() => { });
        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Top));

        Assert.That(GetLabyrinth(rendered).Runner.Position, Is.EqualTo(new Position(1, 0)));
    }

    /// <summary>
    /// Тестирует, что новая партия из диалога финала начинается с чистой живой области и с адресом, указывающим на её зерно, без повторной генерации по этому адресу.
    /// Проверяет, что после кнопки финала объявления прошлой партии стёрты, адрес заменён на текущее зерно с размером и плотностью партии, а доставка нового адреса в страницу не сбрасывает сделанный после этого ход.
    /// </summary>
    /// <param name="buttonText">Надпись кнопки финала</param>
    /// <param name="isSeedKept">Остаётся ли зерно прежним</param>
    [TestCase("Оттачивать навыки в текущем", true)]
    [TestCase("Вернуться в Лабиринт", false)]
    public async Task RestartFromFinaleStartsCleanGameTest(string buttonText, bool isSeedKept)
    {
        const int size = 2;

        var rendered = await RenderMazeAsync(InitialSeed, size);
        var interceptor = rendered.FindComponent<KeyInterceptor>();
        var dialogs = _context.Services.GetRequiredService<DialogService>();
        var navigation = _context.Services.GetRequiredService<NavigationManager>();
        var host = _context.Render<DialogHost>();

        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Right));
        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Bottom));
        rendered.WaitForState(() => dialogs.Instances.Count == 1, WaitTimeout);

        var announcementsBefore = GetAnnouncements(rendered);

        await host.FindAll("button").Single(button => button.TextContent.Contains(buttonText)).ClickAsync(new());
        rendered.WaitForState(() => IsReady(rendered) && dialogs.Instances.Count == 0, WaitTimeout);

        var seed = rendered.FindComponent<RandomGenerator>().Instance.Source.CurrentSeed.ToString(CultureInfo.InvariantCulture);
        var announcementsAfter = GetAnnouncements(rendered);
        var uri = navigation.Uri;

        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Right));
        rendered.WaitForState(() => GetMoveCount(rendered) == "1", WaitTimeout);
        rendered.Render(parameters => parameters.Add(maze => maze.Seed, seed));
        await rendered.InvokeAsync(() => { });

        Assert.Multiple(() =>
        {
            Assert.That(announcementsBefore, Does.Contain("Ряд"));
            Assert.That(announcementsAfter.Trim(), Is.Empty);
            Assert.That(seed == InitialSeed, Is.EqualTo(isSeedKept));
            Assert.That(uri, Is.EqualTo($"{navigation.BaseUri}labirint/{seed}?s={size}&d=0"));
            Assert.That(GetMoveCount(rendered), Is.EqualTo("1"));
            Assert.That(GetLabyrinth(rendered).Runner.Position, Is.EqualTo(new Position(1, 0)));
        });
    }

    /// <summary>
    /// Тестирует, что замена адреса после «Вернуться в Лабиринт» не переносит случайное зерно партии в поле зерна.
    /// Проверяет, что после доставки нового адреса поле зерна пустое, а следующее «Генерировать» даёт другое зерно и переписывает адрес на него.
    /// </summary>
    [Test]
    public async Task ReturnFromFinaleKeepsSeedFieldEmptyTest()
    {
        const int size = 2;

        var rendered = await RenderMazeAsync(InitialSeed, size);
        var interceptor = rendered.FindComponent<KeyInterceptor>();
        var dialogs = _context.Services.GetRequiredService<DialogService>();
        var host = _context.Render<DialogHost>();

        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Right));
        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Bottom));
        rendered.WaitForState(() => dialogs.Instances.Count == 1, WaitTimeout);

        await host.FindAll("button").Single(button => button.TextContent.Contains("Вернуться в Лабиринт")).ClickAsync(new());
        rendered.WaitForState(() => IsReady(rendered) && dialogs.Instances.Count == 0, WaitTimeout);

        var firstSeed = GetSeed(rendered);
        await DeliverRouteAsync(rendered, firstSeed);

        var generator = rendered.FindComponent<RandomGenerator>();
        var userSeed = generator.Instance.Source.UserSeed;
        var fieldValue = generator.Find("input").GetAttribute("value");

        await ClickGenerateAsync(rendered);

        var secondSeed = GetSeed(rendered);

        Assert.Multiple(() =>
        {
            Assert.That(userSeed, Is.Null);
            Assert.That(fieldValue, Is.Null.Or.Empty);
            Assert.That(secondSeed, Is.Not.EqualTo(firstSeed));
            Assert.That(GetUri(), Is.EqualTo(BuildUri(secondSeed, size, 0)));
        });
    }

    /// <summary>
    /// Тестирует, что партия, открытая по адресу без зерна, получает адрес со своим зерном, а переход по ссылке «Лабиринт» даёт новую случайную партию.
    /// Проверяет, что каждый переход доставляется в страницу, как это делает Router, и адрес меняется ровно по разу на партию: после открытия – на зерно, размер и плотность по умолчанию без повторной генерации и без заполнения поля зерна, после возврата на адрес без зерна – на новое зерно.
    /// </summary>
    [Test]
    public async Task RouteWithoutSeedPointsToCurrentGameTest()
    {
        const int defaultSize = 16;
        const int defaultDensity = 40;

        var navigation = _context.Services.GetRequiredService<NavigationManager>();
        var withoutSeedUri = $"{navigation.BaseUri}labirint";
        List<string> locations = [];
        IRenderedComponent<Maze>? routed = null;

        navigation.LocationChanged += (_, args) =>
        {
            locations.Add(args.Location);
            routed?.Render(parameters => parameters.Add(maze => maze.Seed, GetRouteSeed(args.Location)));
        };

        NavigateWithoutSeed();
        var rendered = _context.Render<Maze>();
        routed = rendered;

        rendered.WaitForState(() => IsReady(rendered) && locations.Count == 2, WaitTimeout);
        await rendered.InvokeAsync(() => { });

        var firstSeed = GetSeed(rendered);
        var userSeed = rendered.FindComponent<RandomGenerator>().Instance.Source.UserSeed;

        NavigateWithoutSeed();
        rendered.WaitForState(() => IsReady(rendered) && locations.Count >= 4, WaitTimeout);
        await Task.Delay(100);
        await rendered.InvokeAsync(() => { });

        var secondSeed = GetSeed(rendered);

        Assert.Multiple(() =>
        {
            Assert.That(userSeed, Is.Null);
            Assert.That(secondSeed, Is.Not.EqualTo(firstSeed));
            Assert.That(locations, Is.EqualTo(new[]
            {
                withoutSeedUri,
                BuildUri(firstSeed, defaultSize, defaultDensity),
                withoutSeedUri,
                BuildUri(secondSeed, defaultSize, defaultDensity),
            }));
        });
    }

    /// <summary>
    /// Тестирует, что настоящий Router приложения доставляет смену адреса в открытую страницу Maze, а не пересоздаёт её вместе с раскладкой.
    /// Проверяет, что после перехода на другое зерно и размер и после замены адреса по «Генерировать» страница остаётся тем же экземпляром, лабиринт получает новый размер, адрес – новые зерно и размер, а фокус на заголовок ставится один раз – при открытии страницы.
    /// </summary>
    [Test]
    public async Task RouterKeepsMazeInstanceTest()
    {
        var app = RenderApp(InitialSeed, InitialSize);
        var rendered = app.FindComponent<Maze>();
        var page = rendered.Instance;

        Navigate("2", 8);
        app.WaitForState(() => IsReady(rendered) && GetLabyrinth(rendered).Width == 8, WaitTimeout);
        var routedPage = app.FindComponent<Maze>().Instance;

        await SetNumberAsync(rendered, "Размер", 10);
        await ClickGenerateAsync(rendered);
        app.WaitForState(() => IsReady(rendered) && GetLabyrinth(rendered).Width == 10, WaitTimeout);
        await rendered.InvokeAsync(() => { });

        Assert.Multiple(() =>
        {
            Assert.That(routedPage, Is.SameAs(page));
            Assert.That(app.FindComponent<Maze>().Instance, Is.SameAs(page));
            Assert.That(GetSeed(rendered), Is.EqualTo("2"));
            Assert.That(GetUri(), Is.EqualTo(BuildUri("2", 10, 0)));
            Assert.That(_context.JSInterop.Invocations[FocusBySelector], Has.Count.EqualTo(1));
        });
    }

    /// <summary>
    /// Тестирует, что уход со страницы во время генерации прерывает её и не возвращает игрока в лабиринт заменой адреса.
    /// Проверяет, что после перехода на главную посреди генерации 500×500 по «Генерировать» адрес остаётся адресом главной, а страница Maze не появляется снова.
    /// </summary>
    [Test]
    public async Task LeavingPageCancelsGenerationTest()
    {
        var app = RenderApp(InitialSeed, InitialSize);
        var rendered = app.FindComponent<Maze>();

        await SetNumberAsync(rendered, "Размер", SlowGenerationSize);
        var generation = rendered.FindAll("button").Single(button => button.TextContent.Contains("Генерировать")).ClickAsync(new());

        Assert.That(generation.IsCompleted, Is.False);

        NavigateHome();
        await generation;
        await app.InvokeAsync(() => { });

        Assert.Multiple(() =>
        {
            Assert.That(GetUri(), Is.EqualTo(GetBaseUri()));
            Assert.That(app.FindComponents<Maze>(), Is.Empty);
        });
    }

    /// <summary>
    /// Тестирует, что уход со страницы Maze закрывает открытые ею диалоги – и диалог финала самой страницы, и диалог «Поделиться» её генератора зерна.
    /// Проверяет, что после перехода на главную диалогов не осталось, ShowAsync завершился отменой, а его продолжение не вернуло игрока в лабиринт: адрес – главной, страницы Maze нет.
    /// </summary>
    /// <param name="isFinale">Открыт ли диалог финала, а не «Поделиться»</param>
    [TestCase(true)]
    [TestCase(false)]
    public async Task LeavingPageClosesDialogsTest(bool isFinale)
    {
        var app = RenderApp(InitialSeed, 2);
        var dialogs = _context.Services.GetRequiredService<DialogService>();

        var opening = isFinale
            ? ReachExitAsync(app)
            : app.Find("button[title='Поделиться лабиринтом']").ClickAsync(new());

        app.WaitForState(() => dialogs.Instances.Count == 1, WaitTimeout);
        var dialog = dialogs.Instances.Single();

        NavigateHome();
        app.WaitForState(() => dialogs.Instances.Count == 0, WaitTimeout);
        var result = await dialog.Result;
        await opening.WaitAsync(WaitTimeout);
        await app.InvokeAsync(() => { });

        Assert.Multiple(() =>
        {
            Assert.That(dialog.ContentType, Is.EqualTo(isFinale ? typeof(WinDialog) : typeof(ShareDialog)));
            Assert.That(result.Canceled, Is.True);
            Assert.That(app.FindAll(".ui-dialog-panel"), Is.Empty);
            Assert.That(GetUri(), Is.EqualTo(GetBaseUri()));
            Assert.That(app.FindComponents<Maze>(), Is.Empty);
        });
    }

    /// <summary>
    /// Тестирует, что смена зерна в адресе на открытой странице не закрывает диалог финала: страница та же, уход не состоялся.
    /// Проверяет, что после перехода на другое зерно и перегенерации лабиринта экземпляр страницы прежний, а тот же диалог финала открыт и его ShowAsync не завершён.
    /// </summary>
    [Test]
    public async Task RouteChangeKeepsDialogTest()
    {
        var app = RenderApp(InitialSeed, 2);
        var rendered = app.FindComponent<Maze>();
        var dialogs = _context.Services.GetRequiredService<DialogService>();

        await ReachExitAsync(app);
        app.WaitForState(() => dialogs.Instances.Count == 1, WaitTimeout);
        var dialog = dialogs.Instances.Single();

        Navigate("2", 2);
        app.WaitForState(() => IsReady(rendered) && GetSeed(rendered) == "2", WaitTimeout);
        await app.InvokeAsync(() => { });

        Assert.Multiple(() =>
        {
            Assert.That(app.FindComponent<Maze>().Instance, Is.SameAs(rendered.Instance));
            Assert.That(dialogs.Instances, Is.EqualTo(new[] { dialog }));
            Assert.That(dialog.Result.IsCompleted, Is.False);
        });
    }

    /// <summary>
    /// Тестирует, что ссылка «Поделиться» ведёт через корень сайта и открывает ту же партию.
    /// Проверяет, что ссылка – корень с зерном, размером и плотностью в параметрах, а переход по ней заменяет адрес на страницу лабиринта с теми же зерном, размером и плотностью и даёт те же стены.
    /// </summary>
    [Test]
    public void ShareLinkOpensSameGameTest()
    {
        const int size = 8;
        const int density = 30;

        var app = RenderApp(InitialSeed, size, density);
        var page = app.FindComponent<Maze>();
        var pageInstance = page.Instance;
        var walls = GetWalls(GetLabyrinth(page));
        var link = page.FindComponent<RandomGenerator>().Instance.Link;

        _context.Services.GetRequiredService<NavigationManager>().NavigateTo(link);

        app.WaitForState(() => app.FindComponents<Maze>() is [{ } routed] && routed.Instance != pageInstance && IsReady(routed), WaitTimeout);
        var opened = app.FindComponent<Maze>();

        Assert.Multiple(() =>
        {
            Assert.That(link, Is.EqualTo($"{GetBaseUri()}?seed={InitialSeed}&s={size}&d={density}"));
            Assert.That(GetUri(), Is.EqualTo(BuildUri(InitialSeed, size, density)));
            Assert.That(GetSeed(opened), Is.EqualTo(InitialSeed));
            Assert.That(GetWalls(GetLabyrinth(opened)), Is.EqualTo(walls));
        });
    }

    /// <summary>
    /// Тестирует, что смена адреса посреди генерации перезапускает её по новому адресу, а прерванная генерация не трогает ни лабиринт, ни адрес.
    /// Проверяет, что после перехода на другое зерно, когда генерация 500×500 уже взяла прежнее зерно и сообщает прогресс, зерно партии и адрес – новые, а стены совпадают с лабиринтом, сгенерированным с нуля по новому зерну.
    /// </summary>
    /// <param name="isRegeneration">Идёт ли перегенерация по «Генерировать», а не первая генерация страницы</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task RouteChangeDuringGenerationWinsTest(bool isRegeneration)
    {
        const string nextSeed = "2";
        const int density = 40;

        IRenderedComponent<Maze> rendered;
        var generation = Task.CompletedTask;

        if (isRegeneration)
        {
            rendered = await RenderMazeAsync(InitialSeed, InitialSize, density);
            await SetNumberAsync(rendered, "Размер", SlowGenerationSize);
            generation = rendered.FindAll("button").Single(button => button.TextContent.Contains("Генерировать")).ClickAsync(new());
        }
        else
        {
            Navigate(InitialSeed, SlowGenerationSize, density);
            rendered = _context.Render<Maze>(parameters => parameters.Add(maze => maze.Seed, InitialSeed));
        }

        rendered.WaitForState(() => rendered.Markup.Contains("Генерация.."), WaitTimeout);

        Navigate(nextSeed, SlowGenerationSize, density);
        rendered.Render(parameters => parameters.Add(maze => maze.Seed, nextSeed));

        await generation;
        rendered.WaitForState(() => IsReady(rendered) && GetSeed(rendered) == nextSeed, WaitTimeout);
        await rendered.InvokeAsync(() => { });

        SeedSource source = new() { UserSeed = nextSeed };
        Labyrinth expected = new(source);
        source.Reload();
        expected.Init(SlowGenerationSize, SlowGenerationSize, density);

        Assert.Multiple(() =>
        {
            Assert.That(GetUri(), Is.EqualTo(BuildUri(nextSeed, SlowGenerationSize, density)));
            Assert.That(GetWalls(GetLabyrinth(rendered)), Is.EqualTo(GetWalls(expected)));
        });
    }

    /// <summary>
    /// Тестирует, что «Генерировать» с тем же зерном, но другим размером или плотностью переписывает адрес.
    /// Проверяет, что зерно партии остаётся прежним, а адрес несёт новые размер и плотность.
    /// </summary>
    /// <param name="size">Размер, выставленный в параметрах</param>
    /// <param name="density">Плотность, выставленная в параметрах</param>
    [TestCase(8, 0)]
    [TestCase(InitialSize, 30)]
    public async Task ParameterChangeReplacesRouteTest(int size, int density)
    {
        var rendered = await RenderMazeAsync(InitialSeed, InitialSize);

        await SetNumberAsync(rendered, "Размер", size);
        await SetNumberAsync(rendered, "Плотность", density);
        await ClickGenerateAsync(rendered);

        Assert.Multiple(() =>
        {
            Assert.That(GetSeed(rendered), Is.EqualTo(InitialSeed));
            Assert.That(GetUri(), Is.EqualTo(BuildUri(InitialSeed, size, density)));
        });
    }

    /// <summary>
    /// Тестирует, что словесное зерно попадает в адрес как есть, а слово из одних точек заменяется числом зерна.
    /// Проверяет, что адрес несёт ожидаемый сегмент, а после доставки адреса поле зерна хранит введённое слово и число зерна не меняется.
    /// </summary>
    /// <param name="word">Слово, введённое в поле зерна</param>
    /// <param name="isWordInRoute">Попадает ли слово в адрес без замены числом</param>
    [TestCase(".", false)]
    [TestCase("..", false)]
    [TestCase("abc", true)]
    public async Task WordSeedRouteTest(string word, bool isWordInRoute)
    {
        var rendered = await RenderMazeAsync(InitialSeed, InitialSize);
        var generator = rendered.FindComponent<RandomGenerator>();
        var expectedSeed = SeedSource.ParseSeed(word).ToString(CultureInfo.InvariantCulture);
        var routeSeed = isWordInRoute ? word : expectedSeed;

        await generator.Find("input").InputAsync(new() { Value = word });
        await ClickGenerateAsync(rendered);

        var uri = GetUri();
        await DeliverRouteAsync(rendered, routeSeed);

        Assert.Multiple(() =>
        {
            Assert.That(uri, Is.EqualTo(BuildUri(routeSeed, InitialSize, 0)));
            Assert.That(generator.Instance.Source.UserSeed, Is.EqualTo(word));
            Assert.That(GetSeed(rendered), Is.EqualTo(expectedSeed));
        });
    }

    private static string GetSeed(IRenderedComponent<Maze> rendered)
    {
        return rendered.FindComponent<RandomGenerator>().Instance.Source.CurrentSeed.ToString(CultureInfo.InvariantCulture);
    }

    private static string? GetRouteSeed(string location)
    {
        var segments = new Uri(location).AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 1 ? Uri.UnescapeDataString(segments[1]) : null;
    }

    private static async Task ReachExitAsync(IRenderedComponent<App> app)
    {
        var interceptor = app.FindComponent<KeyInterceptor>();

        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Right));
        await interceptor.InvokeAsync(() => interceptor.Instance.OnKeyDown(Direction.Bottom));
    }

    private static async Task ClickGenerateAsync(IRenderedComponent<Maze> rendered)
    {
        await rendered.FindAll("button").Single(button => button.TextContent.Contains("Генерировать")).ClickAsync(new());
        await rendered.InvokeAsync(() => { });
    }

    private static async Task SetNumberAsync(IRenderedComponent<Maze> rendered, string label, int value)
    {
        var field = rendered.FindComponents<NumberField>().Single(field => field.Instance.Label == label);
        await field.InvokeAsync(() => field.Instance.ValueChanged.InvokeAsync(value));
    }

    private static async Task DeliverRouteAsync(IRenderedComponent<Maze> rendered, string seed)
    {
        rendered.Render(parameters => parameters.Add(maze => maze.Seed, seed));
        await rendered.InvokeAsync(() => { });
    }

    private static Labyrinth GetLabyrinth(IRenderedComponent<Maze> rendered)
    {
        return rendered.FindComponent<MazeField>().Instance.Parameters.Maze;
    }

    private static Direction[] GetWalls(Labyrinth labyrinth)
    {
        var walls = new Direction[labyrinth.Width * labyrinth.Height];

        for (var x = 0; x < labyrinth.Width; x++)
        {
            for (var y = 0; y < labyrinth.Height; y++)
            {
                walls[x * labyrinth.Height + y] = labyrinth[x, y].Walls;
            }
        }

        return walls;
    }

    private static bool IsReady(IRenderedComponent<Maze> rendered)
    {
        return rendered.FindAll(".loading").Count == 0 && rendered.FindComponents<MazeField>().Count == 1;
    }

    private static string GetMoveCount(IRenderedComponent<Maze> rendered)
    {
        return rendered.Find(".stat__number--dim").TextContent.Trim();
    }

    private static string GetAnnouncements(IRenderedComponent<Maze> rendered)
    {
        return rendered.Find("[aria-live]").TextContent;
    }

    private static Bomb GiveBomb(Inventory inventory)
    {
        var bomb = inventory.AllItems.OfType<Bomb>().Single();
        inventory.TryAdd(bomb);
        return bomb;
    }

    private static int GetCount(Inventory inventory, Item item)
    {
        return inventory.Stacks.Single(stack => stack.Item == item).Count;
    }

    private static Func<int> CountUses(Inventory inventory)
    {
        var count = 0;
        inventory.ItemUsed += (_, _) => count++;
        inventory.ItemCantUsed += (_, _) => count++;
        return () => count;
    }

    private static Task TogglePauseAsync(IRenderedComponent<KeyInterceptor> interceptor)
    {
        return interceptor.Find("button").ClickAsync(new());
    }

    private static async Task WaitForCastAsync(IRenderedComponent<Maze> rendered)
    {
        await Task.Delay(AnimatedStackExtensions.UseFlightDuration * 2);
        await rendered.InvokeAsync(() => { });
    }

    private string GetActivateKey(Item item)
    {
        return _context.Services.GetRequiredService<ControlSchemeService>().CurrentScheme.GetActivateKey(item.ControlSettings!).KeyCode;
    }

    private void Navigate(string seed, int size, int density = 0)
    {
        _context.Services.GetRequiredService<NavigationManager>().NavigateTo($"labirint/{seed}?s={size}&d={density}");
    }

    private void NavigateWithoutSeed()
    {
        _context.Services.GetRequiredService<NavigationManager>().NavigateTo("labirint");
    }

    private void NavigateHome()
    {
        _context.Services.GetRequiredService<NavigationManager>().NavigateTo("");
    }

    private string GetUri()
    {
        return _context.Services.GetRequiredService<NavigationManager>().Uri;
    }

    private string GetBaseUri()
    {
        return _context.Services.GetRequiredService<NavigationManager>().BaseUri;
    }

    private IRenderedComponent<App> RenderApp(string seed, int size, int density = 0)
    {
        _context.Services.AddScoped<ThemeService>();
        _context.Services.AddScoped<AppUpdateService>();

        Navigate(seed, size, density);

        var app = _context.Render<App>();
        app.WaitForState(() => IsReady(app.FindComponent<Maze>()), WaitTimeout);

        return app;
    }

    private string BuildUri(string seed, int size, int density)
    {
        return $"{_context.Services.GetRequiredService<NavigationManager>().BaseUri}labirint/{seed}?s={size}&d={density}";
    }

    private async Task<IRenderedComponent<Maze>> RenderMazeAsync(string seed, int size, int density = 0)
    {
        Navigate(seed, size, density);

        var rendered = _context.Render<Maze>(parameters => parameters.Add(maze => maze.Seed, seed));
        rendered.WaitForState(() => IsReady(rendered), WaitTimeout);
        await rendered.InvokeAsync(() => { });

        return rendered;
    }
}
