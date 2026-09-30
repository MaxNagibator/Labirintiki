using Labirint.Core;
using Labirint.Core.Items;
using Labirint.Web.Common.Animation;

namespace Labirint.Web.Tests.Animation;

[TestFixture]
public class AnimatedStackTests
{
    /// <summary>
    /// Тестирует, что AnimatedStack.CancelState очищает состояние стека вместе с очередью ожидающих состояний.
    /// Проверяет, что после отмены очередное состояние Used, поставленное в очередь после Waiting, не воспроизводится.
    /// </summary>
    [Test]
    public void CancelDropsQueuedStateTest()
    {
        var stack = CreateStack();

        stack.AddState(AnimatedStack.State.Waiting);
        stack.GetAnimation();
        stack.AddState(AnimatedStack.State.Used);

        stack.CancelState();

        Assert.That(stack.GetAnimation(), Is.Empty);
    }

    /// <summary>
    /// Тестирует, что AnimatedStack по завершении текущей анимации переходит к состоянию, поставленному в очередь.
    /// Проверяет, что после доигрывания Used стек показывает анимацию состояния CantUse, добавленного следом.
    /// </summary>
    [Test]
    public void FinishedStateGivesWayToQueuedStateTest()
    {
        var stack = CreateStack();

        stack.AddState(AnimatedStack.State.Used);
        stack.GetAnimation();
        stack.AddState(AnimatedStack.State.CantUse);

        var timeout = AnimatedStack.State.Used.ToDuration() + AnimatedStack.State.Used.ToDelay() + 2000;

        Assert.That(() => stack.GetAnimation(),
            Is.EqualTo(AnimatedStack.State.CantUse.ToAnimation()).After(timeout).PollEvery(20));
    }

    /// <summary>
    /// Тестирует, что AnimatedStack.StateChanged остаётся событием экземпляра, а не общим для всех стеков.
    /// Проверяет, что смена состояния у чужого стека не поднимает событие у наблюдаемого.
    /// </summary>
    /// <remarks>Со статическим событием осиротевшие стеки продолжали просить перерисовку.</remarks>
    [Test]
    public void StateChangedIsRaisedByOwnStackOnlyTest()
    {
        var observed = CreateStack();
        var other = CreateStack();

        var count = 0;
        observed.StateChanged += _ => count++;

        other.AddState(AnimatedStack.State.Used);
        other.GetAnimation();

        Assert.That(count, Is.Zero);
    }

    private static AnimatedStack CreateStack()
    {
        return new(new ItemStack(new Hammer()));
    }
}
