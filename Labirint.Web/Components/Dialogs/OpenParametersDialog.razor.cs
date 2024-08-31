using Blazored.LocalStorage;
using Labirint.Web.Parameters;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Labirint.Web.Components.Dialogs;

public partial class OpenParametersDialog
{
    private bool _isProcessing;
    private ControlSchemeSwitcher _controlScheme = null!;

    [CascadingParameter]
    public required MudDialogInstance MudDialog { get; set; }

    [Inject]
    public required ISnackbar SnackbarService { get; set; }

    [Inject]
    public required ILocalStorageService LocalStorage { get; set; }

    private async Task UpdateAsync()
    {
        _isProcessing = true;

        try
        {
            await LocalStorage.SetItemAsync(LabyrinthParameters.LocalStorageKey, GlobalParameters.Labyrinth);
            SnackbarService.Add("Сохранено!", Severity.Success);
            MudDialog.Close();
        }
        catch
        {
            SnackbarService.Add("Не удалось сохранить", Severity.Error);
        }

        _isProcessing = false;
    }

    private void Reset()
    {
        GlobalParameters.Labyrinth = new LabyrinthParameters();
        _controlScheme.ControlSchemeService.Reset();
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}
