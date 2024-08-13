﻿using LabirintBlazorApp.Services.Base;

namespace LabirintBlazorApp.Services;

public class ClipboardService(IJSRuntime jsRuntime) : IClipboardService
{
    public ValueTask CopyToClipboard(string text)
    {
        return jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}
