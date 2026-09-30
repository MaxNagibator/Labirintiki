using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Labirint.Web.Components.Ui;

public partial class SwipeArea : ComponentBase
{
    private double _startX;
    private double _startY;
    private bool _tracking;

    [Parameter]
    [EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    [Parameter]
    public EventCallback<Direction> OnSwipe { get; set; }

    [Parameter]
    public int Threshold { get; set; } = 30;

    [Parameter]
    public string? Class { get; set; }

    private void OnTouchStart(TouchEventArgs args)
    {
        TouchPoint? touch = GetTouch(args);

        if (touch is null)
        {
            _tracking = false;
            return;
        }

        _startX = touch.ClientX;
        _startY = touch.ClientY;
        _tracking = true;
    }

    private async Task OnTouchEnd(TouchEventArgs args)
    {
        if (_tracking == false)
        {
            return;
        }

        _tracking = false;

        TouchPoint? touch = GetTouch(args);

        if (touch is null)
        {
            return;
        }

        double deltaX = touch.ClientX - _startX;
        double deltaY = touch.ClientY - _startY;

        Direction direction = GetDirection(deltaX, deltaY);

        if (direction == Direction.None)
        {
            return;
        }

        await OnSwipe.InvokeAsync(direction);
    }

    private Direction GetDirection(double deltaX, double deltaY)
    {
        if (Math.Abs(deltaX) < Threshold && Math.Abs(deltaY) < Threshold)
        {
            return Direction.None;
        }

        if (Math.Abs(deltaX) >= Math.Abs(deltaY))
        {
            return deltaX > 0 ? Direction.Right : Direction.Left;
        }

        return deltaY > 0 ? Direction.Bottom : Direction.Top;
    }

    private static TouchPoint? GetTouch(TouchEventArgs args)
    {
        TouchPoint[]? touches = args.ChangedTouches;

        return touches is null or { Length: 0 } ? null : touches[^1];
    }
}
