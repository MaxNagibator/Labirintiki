namespace Labirint.Web.Services.Dialogs;

public sealed record DialogResult(bool Canceled, object? Data)
{
    public static DialogResult Ok(object? data = null)
    {
        return new(false, data);
    }

    public static DialogResult Cancelled()
    {
        return new(true, null);
    }

    public T? GetValue<T>()
    {
        return Data is T value ? value : default;
    }
}
