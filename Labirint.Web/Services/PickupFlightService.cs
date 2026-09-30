using Labirint.Web.Common.Animation;

namespace Labirint.Web.Services;

public class PickupFlightService(IJSRuntime jsRuntime)
{
    public ValueTask FlyAsync(Item item)
    {
        return InvokeAsync("labirintPickup.fly", item, AnimatedStackExtensions.PickupFlightDuration);
    }

    public ValueTask CastAsync(Item item)
    {
        return InvokeAsync("labirintPickup.cast", item, AnimatedStackExtensions.UseFlightDuration);
    }

    private async ValueTask InvokeAsync(string identifier, Item item, int duration)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync(identifier, item.Name, item.Icon, duration);
        }
        catch (JSException)
        {
        }
    }
}
