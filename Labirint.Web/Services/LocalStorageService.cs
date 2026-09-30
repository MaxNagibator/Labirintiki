using System.Text.Json;

namespace Labirint.Web.Services;

public sealed class LocalStorageService(IJSRuntime jsRuntime)
{
    public async ValueTask<T?> GetItemAsync<T>(string key)
    {
        string? json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException exception) when (typeof(T) == typeof(string) && exception.Path == "$")
        {
            return (T)(object)json;
        }
    }

    public ValueTask SetItemAsync<T>(string key, T value)
    {
        return jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
    }
}
