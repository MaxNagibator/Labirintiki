using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Labirint.Web.Components.Dialogs;

public partial class WinDialog
{
    [CascadingParameter]
    public required MudDialogInstance MudDialog { get; set; }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}
