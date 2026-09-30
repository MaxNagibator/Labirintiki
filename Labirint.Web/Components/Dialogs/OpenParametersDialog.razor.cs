using Labirint.Web.Common.Control.Schemes;
using Labirint.Web.Parameters;
using Labirint.Web.Services;
using Labirint.Web.Services.Dialogs;
using Labirint.Web.Services.Toasts;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components.Dialogs;

public partial class OpenParametersDialog
{
    private bool _isProcessing;
    private IControlScheme _controlScheme = null!;
    private LabyrinthParameters _parameters = new();

    [CascadingParameter]
    public required DialogInstance Instance { get; set; }

    [Inject]
    public required ToastService ToastService { get; set; }

    [Inject]
    public required LabyrinthParametersService ParametersService { get; set; }

    [Inject]
    public required ControlSchemeService ControlSchemeService { get; set; }

    protected override void OnInitialized()
    {
        _parameters = ParametersService.Current.Clone();
        _controlScheme = ControlSchemeService.CurrentScheme;
    }

    private async Task UpdateAsync()
    {
        _isProcessing = true;

        try
        {
            if (ControlSchemeService.CurrentScheme != _controlScheme)
            {
                await ControlSchemeService.SetSchemeAsync(_controlScheme);
            }

            await ParametersService.SaveAsync(_parameters);

            ToastService.Show("Сохранено!", UiSeverity.Success);
            Instance.Close();
        }
        catch
        {
            ToastService.Show("Не удалось сохранить", UiSeverity.Error);
        }

        _isProcessing = false;
    }

    private void Reset()
    {
        _parameters = new();
        _controlScheme = ControlSchemeService.DefaultScheme;
    }

    private void Cancel()
    {
        Instance.Cancel();
    }
}
