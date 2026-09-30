namespace Labirint.Web.Common.Animation;

public static class AnimatedStackExtensions
{
    public const int PickupFlightDuration = 420;
    public const int UseFlightDuration = 300;

    public static string ToAnimation(this AnimatedStack.State state)
    {
        return state switch
        {
            AnimatedStack.State.Added => "added-animate",
            AnimatedStack.State.Used => "used-animate",
            AnimatedStack.State.CantAdd => "max-count-animate",
            AnimatedStack.State.CantUse => "cant-use-animate",
            AnimatedStack.State.Waiting => "waiting-animate",
            AnimatedStack.State.Removed => string.Empty,
            AnimatedStack.State.None => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };
    }

    public static int ToDuration(this AnimatedStack.State state)
    {
        return state switch
        {
            AnimatedStack.State.Added => 420,
            AnimatedStack.State.Used => 360,
            AnimatedStack.State.CantAdd => 720,
            AnimatedStack.State.CantUse => 600,
            AnimatedStack.State.Waiting => 1200,
            AnimatedStack.State.Removed => 0,
            AnimatedStack.State.None => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };
    }

    public static int ToDelay(this AnimatedStack.State state)
    {
        return state switch
        {
            AnimatedStack.State.Added => PickupFlightDuration,
            AnimatedStack.State.Used => 0,
            AnimatedStack.State.CantAdd => 0,
            AnimatedStack.State.CantUse => 0,
            AnimatedStack.State.Waiting => 0,
            AnimatedStack.State.Removed => 0,
            AnimatedStack.State.None => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };
    }

    public static bool IsRepeating(this AnimatedStack.State state)
    {
        return state is AnimatedStack.State.Waiting;
    }
}
