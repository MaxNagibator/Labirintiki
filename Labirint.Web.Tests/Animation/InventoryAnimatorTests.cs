using Labirint.Core;
using Labirint.Core.Items;
using Labirint.Core.Items.Base;
using Labirint.Web.Common.Animation;

namespace Labirint.Web.Tests.Animation;

[TestFixture]
public class InventoryAnimatorTests
{
    private Inventory _inventory = null!;
    private InventoryAnimator _animator = null!;
    private Labyrinth _labyrinth = null!;

    private Hammer _hammer = null!;
    private Bomb _bomb = null!;
    private WalkThroughWallsBottle _bottle = null!;

    [SetUp]
    public void SetUp()
    {
        _inventory = new();
        _animator = new(_inventory);

        _labyrinth = new(new SeedRandom(1));
        _labyrinth.Init(16, 16, 40, _inventory.AllItems);

        _hammer = _inventory.AllItems.OfType<Hammer>().Single();
        _bomb = _inventory.AllItems.OfType<Bomb>().Single();
        _bottle = _inventory.AllItems.OfType<WalkThroughWallsBottle>().Single();
    }

    [TearDown]
    public void TearDown()
    {
        _animator.Dispose();
        _inventory.Clear();
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator.TryStartCast резервирует единицу предмета в стеке на время броска.
    /// Проверяет, что DisplayCount после старта каста уменьшается на единицу относительно фактического количества.
    /// </summary>
    [Test]
    public void CastReservesCountTest()
    {
        var stack = _animator.Find(_hammer)!;
        var count = stack.Stack.Count;

        Assert.That(_animator.TryStartCast(_hammer), Is.True);
        Assert.That(stack.DisplayCount, Is.EqualTo(count - 1));
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator допускает не более одного одновременного броска предмета.
    /// Проверяет, что повторный TryStartCast для того же и для другого предмета возвращает false, пока первый бросок в полёте.
    /// </summary>
    [Test]
    public void SecondCastIsRejectedWhileFirstIsInFlightTest()
    {
        Assert.That(_animator.TryStartCast(_hammer), Is.True);

        Assert.Multiple(() =>
        {
            Assert.That(_animator.TryStartCast(_hammer), Is.False);
            Assert.That(_animator.TryStartCast(_bomb), Is.False);
        });
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator.CancelCast возвращает зарезервированную единицу предмета обратно в стек.
    /// Проверяет, что DisplayCount восстанавливается до исходного значения и новый бросок другого предмета становится доступен.
    /// </summary>
    [Test]
    public void CancelCastRestoresCountTest()
    {
        var stack = _animator.Find(_hammer)!;
        var count = stack.Stack.Count;

        Assert.That(_animator.TryStartCast(_hammer), Is.True);

        _animator.CancelCast(_hammer);

        Assert.Multiple(() =>
        {
            Assert.That(stack.DisplayCount, Is.EqualTo(count));
            Assert.That(_animator.TryStartCast(_bomb), Is.True);
        });
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator.CancelCast отменяет бросок только указанного предмета.
    /// Проверяет, что отмена каста бомбы не снимает резерв и активный бросок молотка.
    /// </summary>
    [Test]
    public void CancelCastOfAnotherItemKeepsCastTest()
    {
        var stack = _animator.Find(_hammer)!;
        var count = stack.Stack.Count;

        Assert.That(_animator.TryStartCast(_hammer), Is.True);

        _animator.CancelCast(_bomb);

        Assert.Multiple(() =>
        {
            Assert.That(stack.DisplayCount, Is.EqualTo(count - 1));
            Assert.That(_animator.TryStartCast(_bomb), Is.False);
        });
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator завершает бросок после фактического применения предмета через Inventory.Use.
    /// Проверяет, что DisplayCount синхронизируется с реальным количеством в стеке и следующий бросок молотка снова доступен.
    /// </summary>
    [Test]
    public void UsedItemFinishesCastTest()
    {
        var stack = _animator.Find(_hammer)!;

        Assert.That(_animator.TryStartCast(_hammer), Is.True);

        _inventory.Use(_hammer, new(0, 0), Direction.Right, _labyrinth);

        Assert.Multiple(() =>
        {
            Assert.That(stack.DisplayCount, Is.EqualTo(stack.Stack.Count));
            Assert.That(_animator.TryStartCast(_hammer), Is.True);
        });
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator завершает бросок и при отказе движка применить предмет.
    /// Проверяет, что после Inventory.Use, не изменившего лабиринт, DisplayCount синхронизируется со стеком и бросок снова доступен.
    /// </summary>
    [Test]
    public void EngineRefusalFinishesCastTest()
    {
        var stack = _animator.Find(_bottle)!;

        Assert.That(_animator.TryStartCast(_bottle), Is.True);

        _inventory.Use(_bottle, new(0, 0), Direction.Right, _labyrinth);

        Assert.Multiple(() =>
        {
            Assert.That(stack.DisplayCount, Is.EqualTo(stack.Stack.Count));
            Assert.That(_animator.TryStartCast(_bottle), Is.True);
        });
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator.SetWaitItem(null) снимает состояние ожидания направления у стека.
    /// Проверяет, что после установки предмета в ожидание, а затем сброса null, анимация стека возвращается к пустой.
    /// </summary>
    [Test]
    public void WaitItemIsCancelledByNullTest()
    {
        var stack = _animator.Find(_hammer)!;

        _animator.SetWaitItem(_hammer);
        Assert.That(stack.GetAnimation(), Is.EqualTo(AnimatedStack.State.Waiting.ToAnimation()));

        _animator.SetWaitItem(null);
        Assert.That(stack.GetAnimation(), Is.Empty);
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator отписывается от старого AnimatedStack при Inventory.Clear.
    /// Проверяет, что состояние, добавленное осиротевшему стеку после очистки, не поднимает событие Changed, а Find возвращает уже новый стек.
    /// </summary>
    [Test]
    public void ClearedInventoryReleasesOldStacksTest()
    {
        var oldStack = _animator.Find(_hammer)!;

        _inventory.Clear();

        var changedCount = 0;
        _animator.Changed += () => changedCount++;

        oldStack.AddState(AnimatedStack.State.Used);

        Assert.Multiple(() =>
        {
            Assert.That(changedCount, Is.Zero);
            Assert.That(_animator.Find(_hammer), Is.Not.SameAs(oldStack));
        });
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator подписывается на новые стеки, созданные после Inventory.Clear.
    /// Проверяет, что добавление состояния свежесозданному стеку поднимает событие Changed.
    /// </summary>
    [Test]
    public void ClearedInventoryTracksNewStacksTest()
    {
        _inventory.Clear();

        var changedCount = 0;
        _animator.Changed += () => changedCount++;

        _animator.Find(_hammer)!.AddState(AnimatedStack.State.Used);

        Assert.That(changedCount, Is.Positive);
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator сбрасывает незавершённый бросок при Inventory.Clear.
    /// Проверяет, что после очистки инвентаря новый бросок того же предмета снова разрешён.
    /// </summary>
    [Test]
    public void ClearedInventoryDropsCastTest()
    {
        Assert.That(_animator.TryStartCast(_hammer), Is.True);

        _inventory.Clear();

        Assert.That(_animator.TryStartCast(_hammer), Is.True);
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator после Dispose перестаёт реагировать на события Inventory.
    /// Проверяет, что добавление предмета в инвентарь не поднимает ни Changed, ни PickupFlightRequested у отписанного аниматора.
    /// </summary>
    [Test]
    public void DisposedAnimatorIgnoresInventoryTest()
    {
        _animator.Dispose();

        var changedCount = 0;
        var flightCount = 0;

        _animator.Changed += () => changedCount++;
        _animator.PickupFlightRequested += _ => flightCount++;

        _inventory.TryAdd(_bottle);

        Assert.Multiple(() =>
        {
            Assert.That(changedCount, Is.Zero);
            Assert.That(flightCount, Is.Zero);
        });
    }

    /// <summary>
    /// Тестирует, что InventoryAnimator запрашивает полёт призрака иконки при подборе предмета в инвентарь.
    /// Проверяет, что событие PickupFlightRequested поднимается с тем же экземпляром предмета, что был добавлен.
    /// </summary>
    [Test]
    public void PickedUpItemRequestsFlightTest()
    {
        Item? flownItem = null;
        _animator.PickupFlightRequested += item => flownItem = item;

        _inventory.TryAdd(_bottle);

        Assert.That(flownItem, Is.SameAs(_bottle));
    }
}
