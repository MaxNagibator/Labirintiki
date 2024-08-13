namespace LabirintBlazorApp.Services.Base;

public interface IClipboardService
{
    ValueTask CopyToClipboard(string text);
}
