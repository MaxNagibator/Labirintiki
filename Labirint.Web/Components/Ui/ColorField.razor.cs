using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components.Ui;

public partial class ColorField
{
    private const string DefaultRgb = "#000000";

    private readonly string _pickerId = $"cf-picker-{Guid.NewGuid():N}";
    private readonly string _hexId = $"cf-hex-{Guid.NewGuid():N}";
    private readonly string _alphaId = $"cf-alpha-{Guid.NewGuid():N}";

    private string _rgb = DefaultRgb;
    private byte _alpha = byte.MaxValue;
    private string _hex = DefaultRgb;
    private string? _emitted;
    private bool _isRepaired;
    private int _hexRevision;

    [Parameter]
    public string Value { get; set; } = DefaultRgb;

    [Parameter]
    public string FallbackValue { get; set; } = DefaultRgb;

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public string? AriaLabel { get; set; }

    [Parameter]
    public bool ShowAlpha { get; set; } = true;

    [Parameter]
    public string? Class { get; set; }

    private string PickerLabel => string.IsNullOrEmpty(Label) ? "Выбор цвета" : $"{Label} – выбор цвета";

    private string AlphaValue => (_alpha / (double)byte.MaxValue).ToString("0.##", CultureInfo.InvariantCulture);

    private string AlphaText => (_alpha / (double)byte.MaxValue).ToString("0%", CultureInfo.InvariantCulture);

    protected override async Task OnParametersSetAsync()
    {
        if (Value == _emitted)
        {
            return;
        }

        if (TryParse(Value, out string rgb, out byte alpha) == false)
        {
            if (_isRepaired)
            {
                return;
            }

            _isRepaired = true;
            TryParse(FallbackValue, out string fallbackRgb, out byte fallbackAlpha);
            _rgb = fallbackRgb;
            _alpha = fallbackAlpha;

            await CommitAsync();
            return;
        }

        _isRepaired = false;
        _rgb = rgb;
        _alpha = alpha;
        _hex = Compose(rgb, alpha);
        _emitted = _hex;
    }

    private static bool TryParse(string? value, out string rgb, out byte alpha)
    {
        rgb = DefaultRgb;
        alpha = byte.MaxValue;

        if (value is null)
        {
            return false;
        }

        string trimmed = value.Trim();

        if (trimmed.StartsWith('#') == false)
        {
            return false;
        }

        string digits = trimmed[1..];

        if (digits.Length != 6 && digits.Length != 8)
        {
            return false;
        }

        foreach (char digit in digits)
        {
            if (Uri.IsHexDigit(digit) == false)
            {
                return false;
            }
        }

        rgb = $"#{digits[..6].ToLowerInvariant()}";

        if (digits.Length == 8)
        {
            alpha = byte.Parse(digits[6..], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return true;
    }

    private static string Compose(string rgb, byte alpha)
    {
        return alpha == byte.MaxValue ? rgb : $"{rgb}{alpha:x2}";
    }

    private async Task OnPickerChangedAsync(ChangeEventArgs args)
    {
        if (TryParse(args.Value?.ToString(), out string rgb, out _) == false)
        {
            return;
        }

        _rgb = rgb;
        await CommitAsync();
    }

    private async Task OnHexChangedAsync(ChangeEventArgs args)
    {
        if (TryParse(args.Value?.ToString(), out string rgb, out byte alpha) == false)
        {
            _hex = Compose(_rgb, _alpha);
            _hexRevision++;
            StateHasChanged();
            return;
        }

        _rgb = rgb;
        _alpha = ShowAlpha ? alpha : byte.MaxValue;
        await CommitAsync();
    }

    private async Task OnAlphaChangedAsync(ChangeEventArgs args)
    {
        if (double.TryParse(args.Value?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed) == false)
        {
            return;
        }

        _alpha = (byte)Math.Clamp(Math.Round(parsed * byte.MaxValue), 0, byte.MaxValue);
        await CommitAsync();
    }

    private async Task CommitAsync()
    {
        _hex = Compose(_rgb, _alpha);

        if (_hex == _emitted)
        {
            return;
        }

        _emitted = _hex;
        Value = _hex;
        await ValueChanged.InvokeAsync(_hex);
    }
}
