namespace Labirint.Web.Services.Base;

public interface IClipboardService
{
    ValueTask CopyToClipboard(string text);
}
