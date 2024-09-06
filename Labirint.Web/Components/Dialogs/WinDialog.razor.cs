using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Labirint.Web.Components.Dialogs;

public partial class WinDialog
{
    [Parameter]
    public required Func<Task> OnRestart { get; set; }

    [Parameter]
    public required RandomGenerator Seeder { get; set; }

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    private void Close()
    {
        MudDialog.Close(true);
    }

    private void RepeatGame()
    {
        NavigationManager.NavigateTo(Seeder.Link);
        MudDialog.Close(false);
    }

    private async Task RestartGameAsync()
    {
        Seeder.Reload(true);
        await OnRestart();

        MudDialog.Close(false);
    }
}
