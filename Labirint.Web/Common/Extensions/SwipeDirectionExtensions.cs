using MudBlazor;

namespace Labirint.Web.Common.Extensions;

public static class SwipeDirectionExtensions
{
    public static Direction ToDirection(this SwipeDirection swipeDirection)
    {
        return swipeDirection switch
        {
            SwipeDirection.None => Direction.None,
            SwipeDirection.LeftToRight => Direction.Right,
            SwipeDirection.RightToLeft => Direction.Left,
            SwipeDirection.TopToBottom => Direction.Bottom,
            SwipeDirection.BottomToTop => Direction.Top,
            var _ => throw new ArgumentOutOfRangeException(nameof(swipeDirection), swipeDirection, null)
        };
    }
}
