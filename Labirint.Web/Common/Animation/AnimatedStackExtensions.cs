namespace Labirint.Web.Common.Animation;

public static class AnimatedStackExtensions
{
    public static string ToAnimation(this AnimatedStack.State state)
    {
        return state switch
        {
            AnimatedStack.State.Added => "added-animate",
            AnimatedStack.State.Used => "used-animate",
            AnimatedStack.State.CantAdd => "max-count-animate",
            AnimatedStack.State.Waiting => "waiting-animate",
            AnimatedStack.State.Removed => string.Empty,
            AnimatedStack.State.None => string.Empty,
            var _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}
