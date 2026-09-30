using Bunit;
using Bunit.TestDoubles;
using Labirint.Web.Pages;
using Labirint.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Labirint.Web.Tests.Pages;

[TestFixture]
public class HomeTests
{
    private BunitContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new();
        _context.JSInterop.Mode = JSRuntimeMode.Loose;
        _context.Services.AddLogging();
        _context.Services.AddScoped<MotionService>();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Тестирует, что главная, открытая по ссылке «Поделиться», переводит игрока на страницу лабиринта с тем же зерном, размером и плотностью.
    /// Проверяет, что адрес заменён без новой записи в истории, нечисловое зерно экранировано в сегменте пути, а отсутствующие или нечисловые размер и плотность в адрес лабиринта не попадают.
    /// </summary>
    /// <param name="query">Параметры ссылки на главную</param>
    /// <param name="expectedPath">Ожидаемый адрес относительно корня сайта</param>
    [TestCase("?seed=12345&s=8&d=30", "labirint/12345?s=8&d=30")]
    [TestCase("?seed=abc", "labirint/abc")]
    [TestCase("?seed=%D0%B7%D0%B5%D1%80%D0%BD%D0%BE%20%2F%3F%26&s=8", "labirint/%D0%B7%D0%B5%D1%80%D0%BD%D0%BE%20%2F%3F%26?s=8")]
    [TestCase("?seed=1&s=abc&d=30", "labirint/1?d=30")]
    public void SharedLinkRedirectsToMazeTest(string query, string expectedPath)
    {
        var navigation = _context.Services.GetRequiredService<BunitNavigationManager>();
        navigation.NavigateTo(query);

        _context.Render<Home>();

        var last = navigation.History.First();

        Assert.Multiple(() =>
        {
            Assert.That(navigation.Uri, Is.EqualTo($"{navigation.BaseUri}{expectedPath}"));
            Assert.That(last.Options.ReplaceHistoryEntry, Is.True);
        });
    }

    /// <summary>
    /// Тестирует, что главная без зерна в адресе остаётся главной.
    /// Проверяет, что адрес не меняется, новых переходов нет и заставка с лабиринтом нарисована.
    /// </summary>
    /// <param name="query">Параметры адреса главной</param>
    [TestCase("")]
    [TestCase("?seed=")]
    [TestCase("?seed=%20")]
    [TestCase("?s=8&d=30")]
    public void HomeWithoutSeedStaysTest(string query)
    {
        var navigation = _context.Services.GetRequiredService<BunitNavigationManager>();
        navigation.NavigateTo(query);
        var uri = navigation.Uri;

        var rendered = _context.Render<Home>();

        Assert.Multiple(() =>
        {
            Assert.That(navigation.Uri, Is.EqualTo(uri));
            Assert.That(navigation.History, Has.Count.EqualTo(1));
            Assert.That(rendered.FindAll(".hero__plate"), Has.Count.EqualTo(1));
        });
    }
}
