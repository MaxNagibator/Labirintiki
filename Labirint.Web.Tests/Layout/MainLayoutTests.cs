using Bunit;
using Labirint.Web.Layout;
using Labirint.Web.Services;
using Labirint.Web.Services.Dialogs;
using Labirint.Web.Services.Toasts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Labirint.Web.Tests.Layout;

[TestFixture]
public class MainLayoutTests
{
    private const string ErrorText = "Что-то сломалось";

    private BunitContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new();
        _context.JSInterop.Mode = JSRuntimeMode.Loose;
        _context.Services.AddLogging();
        _context.Services.AddSingleton<LocalStorageService>();
        _context.Services.AddScoped<ToastService>();
        _context.Services.AddScoped<DialogService>();
        _context.Services.AddScoped<ThemeService>();
        _context.Services.AddScoped<AppUpdateService>();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Тестирует, что раскладка, которая переживает навигации, сбрасывает ErrorBoundary, когда Router приносит ей новое содержимое страницы.
    /// Проверяет, что после сбоя страницы показан текст ошибки, а после смены Body на исправную страницу текст ошибки исчезает и выводится её содержимое.
    /// </summary>
    [Test]
    public void ErrorBoundaryRecoversOnNavigationTest()
    {
        var layout = _context.Render<MainLayout>(parameters => parameters.Add(main => main.Body, builder =>
        {
            builder.OpenComponent<FailingPage>(0);
            builder.CloseComponent();
        }));

        var failedMarkup = layout.Find("main").TextContent;

        layout.Render(parameters => parameters.Add(main => main.Body, builder => builder.AddContent(0, "Главная")));

        var recoveredMarkup = layout.Find("main").TextContent;

        Assert.Multiple(() =>
        {
            Assert.That(failedMarkup, Does.Contain(ErrorText));
            Assert.That(recoveredMarkup, Does.Not.Contain(ErrorText));
            Assert.That(recoveredMarkup, Does.Contain("Главная"));
        });
    }

    private sealed class FailingPage : ComponentBase
    {
        protected override void OnInitialized()
        {
            throw new InvalidOperationException("Сбой страницы");
        }
    }
}
