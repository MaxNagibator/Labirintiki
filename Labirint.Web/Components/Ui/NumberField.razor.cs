using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Labirint.Web.Components.Ui;

public partial class NumberField : IDisposable
{
    private readonly string _id = $"nf-{Guid.NewGuid():N}";

    private CancellationTokenSource? _debounce;
    private string? _text;
    private int? _committed;

    [Parameter]
    public int Value { get; set; }

    [Parameter]
    public EventCallback<int> ValueChanged { get; set; }

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public string? AriaLabel { get; set; }

    [Parameter]
    public int Min { get; set; }

    [Parameter]
    public int Max { get; set; }

    [Parameter]
    public int DebounceMilliseconds { get; set; } = 300;

    [Parameter]
    public string? Class { get; set; }

    public void Dispose()
    {
        _debounce?.Cancel();
        _debounce?.Dispose();
        _debounce = null;

        GC.SuppressFinalize(this);
    }

    protected override void OnParametersSet()
    {
        if (Value == _committed)
        {
            return;
        }

        _committed = Value;
        _text = Value.ToString();
    }

    private async Task OnInputAsync(ChangeEventArgs args)
    {
        _text = args.Value?.ToString();

        _debounce?.Cancel();
        _debounce?.Dispose();
        _debounce = new();

        CancellationToken token = _debounce.Token;

        try
        {
            await Task.Delay(DebounceMilliseconds, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        await CommitAsync();
    }

    private async Task OnBlurAsync(FocusEventArgs args)
    {
        _debounce?.Cancel();
        _debounce?.Dispose();
        _debounce = null;

        await CommitAsync();
    }

    private async Task CommitAsync()
    {
        if (int.TryParse(_text, out int parsed) == false)
        {
            _committed = Value;
            _text = Value.ToString();
            StateHasChanged();
            return;
        }

        int clamped = Math.Clamp(parsed, Min, Max);

        _text = clamped.ToString();
        _committed = clamped;

        if (clamped == Value)
        {
            StateHasChanged();
            return;
        }

        Value = clamped;
        await ValueChanged.InvokeAsync(clamped);
    }
}
