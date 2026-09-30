using Labirint.Web.Common.Ui;

namespace Labirint.Web.Services.Toasts;

public sealed class ToastService
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(3.5);

    private readonly List<Toast> _toasts = [];

    public event Action? Changed;

    public IReadOnlyList<Toast> Toasts => [.. _toasts];

    public void Show(string message, UiSeverity severity = UiSeverity.Info)
    {
        Toast toast = new(Guid.NewGuid(), message, severity);

        _toasts.Add(toast);
        Changed?.Invoke();

        _ = ExpireAsync(toast);
    }

    public void Remove(Toast toast)
    {
        if (_toasts.Remove(toast))
        {
            Changed?.Invoke();
        }
    }

    private async Task ExpireAsync(Toast toast)
    {
        await Task.Delay(Lifetime);
        Remove(toast);
    }
}
