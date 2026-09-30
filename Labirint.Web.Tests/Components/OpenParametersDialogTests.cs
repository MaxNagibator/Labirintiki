using Bunit;
using Labirint.Web.Common.Control.Schemes;
using Labirint.Web.Common.Ui;
using Labirint.Web.Components;
using Labirint.Web.Components.Dialogs;
using Labirint.Web.Parameters;
using Labirint.Web.Services;
using Labirint.Web.Services.Dialogs;
using Labirint.Web.Services.Toasts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Labirint.Web.Tests.Components;

[TestFixture]
public class OpenParametersDialogTests
{
    private BunitContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new();
        _context.JSInterop.Mode = JSRuntimeMode.Loose;
        _context.Services.AddLogging();
        _context.Services.AddSingleton<LocalStorageService>();
        _context.Services.AddSingleton<LabyrinthParametersService>();
        _context.Services.AddSingleton<ControlSchemeService>();
        _context.Services.AddScoped<DialogService>();
        _context.Services.AddScoped<ToastService>();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Тестирует, что OpenParametersDialog при сбое записи схемы управления не сохраняет и остальные параметры.
    /// Проверяет, что параметры лабиринта не уходят в localStorage и не применяются, диалог остаётся открытым, а игрок видит тост «Не удалось сохранить».
    /// </summary>
    [Test]
    public async Task SchemeSaveFailureSavesNothingTest()
    {
        _context.JSInterop.SetupVoid("localStorage.setItem", invocation => Equals(invocation.Arguments[0], nameof(ControlSchemeService)))
            .SetException(new JSException("Хранилище переполнено"));

        var parametersService = _context.Services.GetRequiredService<LabyrinthParametersService>();
        var initialParameters = parametersService.Current;
        var dialogs = _context.Services.GetRequiredService<DialogService>();
        var result = dialogs.ShowAsync<OpenParametersDialog>(this, "Настройки");
        var instance = dialogs.Instances.Single();

        var rendered = _context.Render<OpenParametersDialog>(parameters => parameters.AddCascadingValue(instance));
        var switcher = rendered.FindComponent<ControlSchemeSwitcher>();
        var alternative = _context.Services.GetRequiredService<ControlSchemeService>().AvailableSchemes.OfType<AlternativeScheme>().Single();

        await rendered.InvokeAsync(() => switcher.Instance.ValueChanged.InvokeAsync(alternative));
        await rendered.FindAll("button").Single(button => button.TextContent.Contains("Сохранить")).ClickAsync(new());

        var parameterWrites = _context.JSInterop.Invocations["localStorage.setItem"]
            .Count(invocation => Equals(invocation.Arguments[0], LabyrinthParameters.LocalStorageKey));

        Assert.Multiple(() =>
        {
            Assert.That(parameterWrites, Is.Zero);
            Assert.That(parametersService.Current, Is.SameAs(initialParameters));
            Assert.That(result.IsCompleted, Is.False);
            Assert.That(_context.Services.GetRequiredService<ToastService>().Toasts.Select(toast => (toast.Message, toast.Severity)),
                Is.EqualTo(new[] { ("Не удалось сохранить", UiSeverity.Error) }));
        });
    }
}
