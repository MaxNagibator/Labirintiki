using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components;

public partial class TouchInterceptor
{
    public event EventHandler<Direction>? Moved;

    [Parameter]
    [EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    private void OnZoneClicked(Direction direction)
    {
        Moved?.Invoke(this, direction);
    }

    private void OnSwipe(Direction direction)
    {
        if (direction != Direction.None)
        {
            Moved?.Invoke(this, direction);
        }
    }
}
