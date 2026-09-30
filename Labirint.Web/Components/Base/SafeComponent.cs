using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Labirint.Web.Components.Base;

public abstract class SafeComponent : ComponentBase
{
    private ILogger? _logger;

    [Inject]
    private ILoggerFactory LoggerFactory { get; set; } = null!;

    protected void RunSafe(Func<Task> work)
    {
        _ = InvokeAsync(async () =>
        {
            try
            {
                await work();
            }
            catch (Exception exception)
            {
                _logger ??= LoggerFactory.CreateLogger(GetType());
                _logger.LogError(exception, "Обработчик события завершился ошибкой");
            }
        });
    }
}
