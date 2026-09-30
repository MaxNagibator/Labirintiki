using Labirint.Web.Services.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components.Dialogs;

public partial class WinDialog
{
    [Parameter]
    public required Func<Task> OnRestart { get; set; }

    [Parameter]
    public required Func<Task> OnRepeat { get; set; }

    [Parameter]
    public required RandomGenerator Seeder { get; set; }

    [Parameter]
    public int Score { get; set; }

    [Parameter]
    public int MoveCount { get; set; }

    [Parameter]
    public int Size { get; set; }

    [CascadingParameter]
    private DialogInstance Instance { get; set; } = null!;

    private void Close()
    {
        Instance.Close(true);
    }

    private async Task RepeatGameAsync()
    {
        await OnRepeat();

        Instance.Close(false);
    }

    private async Task RestartGameAsync()
    {
        Seeder.Reload(true);
        await OnRestart();

        Instance.Close(false);
    }
}
