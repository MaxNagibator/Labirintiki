namespace LabirintBlazorApp.Services;

public interface IClipboardService
{
    ValueTask CopyToClipboard(string text);
}
