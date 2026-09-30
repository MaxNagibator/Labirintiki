using Bunit;
using Labirint.Core;
using Labirint.Core.Items;
using Labirint.Core.Items.Base;
using Labirint.Web.Common.Control;
using Labirint.Web.Common.Control.Schemes;
using Labirint.Web.Components;
using Labirint.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Labirint.Web.Tests.Components;

[TestFixture]
public class KeyInterceptorTests
{
    private BunitContext _context = null!;
    private Inventory _inventory = null!;
    private ClassicScheme _scheme = null!;

    private Hammer _hammer = null!;
    private Bomb _bomb = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new();
        _context.JSInterop.Mode = JSRuntimeMode.Loose;
        _context.Services.AddLogging();
        _context.Services.AddSingleton<LocalStorageService>();
        _context.Services.AddSingleton<ControlSchemeService>();

        _inventory = new();
        _scheme = new();

        _hammer = _inventory.AllItems.OfType<Hammer>().Single();
        _bomb = _inventory.AllItems.OfType<Bomb>().Single();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        _inventory.Clear();
    }

    /// <summary>
    /// Тестирует, что KeyInterceptor на паузе игнорирует нажатие клавиши движения.
    /// Проверяет, что событие MoveKeyDown не поднимается, пока IsPaused равен true.
    /// </summary>
    [Test]
    public async Task PausedInterceptorIgnoresKeyMoveTest()
    {
        var rendered = RenderInterceptor();
        var moveCount = 0;

        rendered.Instance.MoveKeyDown += (_, _) => moveCount++;

        TogglePause(rendered);
        await PressKeyAsync(rendered, _scheme.MoveLeft.KeyCode);

        Assert.Multiple(() =>
        {
            Assert.That(rendered.Instance.IsPaused, Is.True);
            Assert.That(moveCount, Is.Zero);
        });
    }

    /// <summary>
    /// Тестирует, что KeyInterceptor.PerformMove на паузе отсекает и свайпы, не только клавиши.
    /// Проверяет, что вызов OnKeyDown(Direction) на паузе не поднимает MoveKeyDown, поскольку проверка паузы стоит в единой точке PerformMove.
    /// </summary>
    [Test]
    public async Task PausedInterceptorIgnoresSwipeTest()
    {
        var rendered = RenderInterceptor();
        var moveCount = 0;

        rendered.Instance.MoveKeyDown += (_, _) => moveCount++;

        TogglePause(rendered);
        await SwipeAsync(rendered, Direction.Left);

        Assert.That(moveCount, Is.Zero);
    }

    /// <summary>
    /// Тестирует, что KeyInterceptor после снятия паузы снова принимает свайпы как ход.
    /// Проверяет, что после двойного переключения паузы (пауза и снятие) свайп поднимает MoveKeyDown ровно один раз.
    /// </summary>
    [Test]
    public async Task ResumedInterceptorAcceptsSwipeTest()
    {
        var rendered = RenderInterceptor();
        var moveCount = 0;

        rendered.Instance.MoveKeyDown += (_, _) => moveCount++;

        TogglePause(rendered);
        TogglePause(rendered);
        await SwipeAsync(rendered, Direction.Left);

        Assert.Multiple(() =>
        {
            Assert.That(rendered.Instance.IsPaused, Is.False);
            Assert.That(moveCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Тестирует, что KeyInterceptor для предмета, требующего направление, сначала ждёт клавишу движения и лишь потом поднимает атаку.
    /// Проверяет, что активация молотка выставляет ChangedWaitItem без AttackKeyDown, а следующая клавиша движения даёт AttackKeyDown с нужным направлением и не поднимает MoveKeyDown.
    /// </summary>
    [Test]
    public async Task MoveRequiredItemWaitsForDirectionTest()
    {
        var rendered = RenderInterceptor();

        AttackEventArgs? attack = null;
        Item? waitItem = null;
        var moveCount = 0;

        rendered.Instance.AttackKeyDown += (_, args) => attack = args;
        rendered.Instance.ChangedWaitItem += (_, item) => waitItem = item;
        rendered.Instance.MoveKeyDown += (_, _) => moveCount++;

        await PressKeyAsync(rendered, _scheme.GetActivateKey(_hammer.ControlSettings!).KeyCode);

        Assert.Multiple(() =>
        {
            Assert.That(waitItem, Is.SameAs(_hammer));
            Assert.That(attack, Is.Null);
        });

        await PressKeyAsync(rendered, _scheme.MoveLeft.KeyCode);

        Assert.Multiple(() =>
        {
            Assert.That(attack?.Item, Is.SameAs(_hammer));
            Assert.That(attack?.Direction, Is.EqualTo(Direction.Left));
            Assert.That(waitItem, Is.Null);
            Assert.That(moveCount, Is.Zero);
        });
    }

    /// <summary>
    /// Тестирует, что KeyInterceptor для мгновенного предмета не требует направления перед атакой.
    /// Проверяет, что активация бомбы сразу поднимает AttackKeyDown с Direction, равным null.
    /// </summary>
    [Test]
    public async Task InstantItemAttacksWithoutDirectionTest()
    {
        var rendered = RenderInterceptor();

        AttackEventArgs? attack = null;
        rendered.Instance.AttackKeyDown += (_, args) => attack = args;

        await PressKeyAsync(rendered, _scheme.GetActivateKey(_bomb.ControlSettings!).KeyCode);

        Assert.Multiple(() =>
        {
            Assert.That(attack?.Item, Is.SameAs(_bomb));
            Assert.That(attack?.Direction, Is.Null);
        });
    }

    /// <summary>
    /// Тестирует, что KeyInterceptor.ResetWaitItem снимает ожидание направления и возвращает клавиши движения обычному ходу.
    /// Проверяет, что после сброса ожидающий предмет становится null, а нажатие клавиши движения поднимает MoveKeyDown вместо AttackKeyDown.
    /// </summary>
    [Test]
    public async Task ResetWaitItemReturnsMovesTest()
    {
        var rendered = RenderInterceptor();

        Item? waitItem = _hammer;
        var moveCount = 0;
        var attackCount = 0;

        rendered.Instance.ChangedWaitItem += (_, item) => waitItem = item;
        rendered.Instance.MoveKeyDown += (_, _) => moveCount++;
        rendered.Instance.AttackKeyDown += (_, _) => attackCount++;

        await PressKeyAsync(rendered, _scheme.GetActivateKey(_hammer.ControlSettings!).KeyCode);
        await rendered.InvokeAsync(rendered.Instance.ResetWaitItem);
        await PressKeyAsync(rendered, _scheme.MoveLeft.KeyCode);

        Assert.Multiple(() =>
        {
            Assert.That(waitItem, Is.Null);
            Assert.That(moveCount, Is.EqualTo(1));
            Assert.That(attackCount, Is.Zero);
        });
    }

    /// <summary>
    /// Тестирует, что KeyInterceptor не начинает ожидание направления для предмета, который CanUse запрещает использовать.
    /// Проверяет, что активация исчерпанной бомбы не поднимает ни AttackKeyDown, ни ChangedWaitItem.
    /// </summary>
    [Test]
    public async Task EmptyItemDoesNotWaitForDirectionTest()
    {
        Labyrinth labyrinth = new(new SeedRandom(1));
        labyrinth.Init(16, 16, 40, _inventory.AllItems);

        while (_inventory.CanUse(_bomb))
        {
            _inventory.Use(_bomb, new(0, 0), Direction.Right, labyrinth);
        }

        var rendered = RenderInterceptor();

        var attackCount = 0;
        var waitCount = 0;

        rendered.Instance.AttackKeyDown += (_, _) => attackCount++;
        rendered.Instance.ChangedWaitItem += (_, _) => waitCount++;

        await PressKeyAsync(rendered, _scheme.GetActivateKey(_bomb.ControlSettings!).KeyCode);

        Assert.Multiple(() =>
        {
            Assert.That(attackCount, Is.Zero);
            Assert.That(waitCount, Is.Zero);
        });
    }

    private static Task PressKeyAsync(IRenderedComponent<KeyInterceptor> rendered, string code)
    {
        return rendered.InvokeAsync(() => rendered.Instance.OnKeyDown(code));
    }

    private static Task SwipeAsync(IRenderedComponent<KeyInterceptor> rendered, Direction direction)
    {
        return rendered.InvokeAsync(() => rendered.Instance.OnKeyDown(direction));
    }

    private static void TogglePause(IRenderedComponent<KeyInterceptor> rendered)
    {
        rendered.Find("button").Click();
    }

    private IRenderedComponent<KeyInterceptor> RenderInterceptor()
    {
        return _context.Render<KeyInterceptor>(parameters => parameters.Add(interceptor => interceptor.Inventory, _inventory));
    }
}
