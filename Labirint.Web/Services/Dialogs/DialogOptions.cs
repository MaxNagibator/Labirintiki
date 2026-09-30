using Labirint.Web.Common.Ui;

namespace Labirint.Web.Services.Dialogs;

public sealed record DialogOptions
{
    public bool CloseButton { get; init; } = true;

    public bool CloseOnBackdropClick { get; init; } = true;

    public bool CloseOnEscape { get; init; } = true;

    public DialogWidth Width { get; init; } = DialogWidth.Medium;
}
