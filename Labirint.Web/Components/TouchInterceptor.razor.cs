using System.Drawing;
using Labirint.Web.Common.Extensions;
using Labirint.Web.Parameters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Direction = Labirint.Core.Common.Direction;

namespace Labirint.Web.Components;

public partial class TouchInterceptor
{
    private Rectangle _left;
    private Rectangle _top;
    private Rectangle _right;
    private Rectangle _bottom;
    private int? _xStep;
    private int? _yStep;

    public event EventHandler<Direction>? Moved;

    [Parameter]
    [EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    [CascadingParameter]
    public required MazeRenderParameters RenderParameters { get; set; }

    protected override void OnParametersSet()
    {
        if (_xStep != null && _yStep != null)
        {
            return;
        }

        int renderRange = RenderParameters.Vision.Range * 2 * RenderParameters.BoxSize + RenderParameters.BoxSize + RenderParameters.WallWidth;
        int xStep = renderRange / 4;
        int yStep = renderRange / 4;

        _left = new Rectangle(0, yStep, xStep, yStep * 2);
        _top = new Rectangle(xStep, 0, xStep * 2, yStep);
        _right = new Rectangle(xStep * 3, yStep, xStep, yStep * 2);
        _bottom = new Rectangle(xStep, yStep * 3, xStep * 2, yStep);

        _xStep = xStep;
        _yStep = yStep;
    }

    private void OnFieldClicked(MouseEventArgs args)
    {
        Direction direction = GetDirection((int)args.OffsetX, (int)args.OffsetY);

        if (direction != Direction.None)
        {
            Moved?.Invoke(this, direction);
        }
    }

    private void OnSwipeEnd(SwipeEventArgs args)
    {
        if (args.SwipeDirection != SwipeDirection.None)
        {
            Moved?.Invoke(this, args.SwipeDirection.ToDirection());
        }
    }

    private Direction GetDirection(int x, int y)
    {
        if (_left.Contains(x, y))
        {
            return Direction.Left;
        }

        if (_top.Contains(x, y))
        {
            return Direction.Top;
        }

        if (_right.Contains(x, y))
        {
            return Direction.Right;
        }

        if (_bottom.Contains(x, y))
        {
            return Direction.Bottom;
        }

        return Direction.None;
    }
}
