using Labirint.Web.Services.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components.Ui;

public partial class Dialog
{
    [CascadingParameter]
    public required DialogInstance Instance { get; set; }

    [Parameter]
    public RenderFragment? TitleContent { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? Actions { get; set; }

    [Parameter]
    public string? Class { get; set; }
}
