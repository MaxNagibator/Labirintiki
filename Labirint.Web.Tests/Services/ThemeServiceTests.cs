using Bunit;
using Labirint.Web.Common.Ui;
using Labirint.Web.Services;
using Labirint.Web.Services.Toasts;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;

namespace Labirint.Web.Tests.Services;

[TestFixture]
public class ThemeServiceTests
{
    private const string StorageKey = "IsDarkMod";

    private BunitContext _context = null!;
    private JSRuntimeInvocationHandler<string?> _readHandler = null!;
    private ToastService _toastService = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new();
        _toastService = new();
        _context.JSInterop.Mode = JSRuntimeMode.Loose;

        _readHandler = _context.JSInterop.Setup<string?>("localStorage.getItem", StorageKey);
    }

    [TearDown]
    public void TearDown()
    {
        _readHandler.Dispose();
        _context.Dispose();
    }

    /// <summary>
    /// Тестирует, что ThemeService.InitializeAsync при отсутствии сохранённого значения берёт тему из системных настроек браузера.
    /// Проверяет, что для системной тёмной и светлой темы IsDark и применённая через labirintTheme.apply тема совпадают с системным значением.
    /// </summary>
    /// <param name="isSystemDark">Тёмная ли системная тема</param>
    /// <param name="expectedTheme">Ожидаемое имя применённой темы</param>
    [TestCase(true, "dark")]
    [TestCase(false, "light")]
    public async Task SystemThemeIsUsedWithoutStoredValueTest(bool isSystemDark, string expectedTheme)
    {
        _readHandler.SetResult(null);
        _context.JSInterop.Setup<bool>("labirintTheme.isSystemDark").SetResult(isSystemDark);

        var service = CreateService();
        await service.InitializeAsync();

        Assert.Multiple(() =>
        {
            Assert.That(service.IsDark, Is.EqualTo(isSystemDark));
            Assert.That(GetAppliedTheme(), Is.EqualTo(expectedTheme));
        });
    }

    /// <summary>
    /// Тестирует, что ThemeService.InitializeAsync отдаёт приоритет значению из localStorage перед системной темой.
    /// Проверяет, что сохранённое значение "true" или "false" определяет IsDark даже когда системная тема ему противоположна.
    /// </summary>
    /// <param name="stored">Значение под ключом IsDarkMod в localStorage</param>
    /// <param name="expectedIsDark">Ожидаемое значение IsDark</param>
    [TestCase("true", true)]
    [TestCase("false", false)]
    public async Task StoredThemeWinsOverSystemTest(string stored, bool expectedIsDark)
    {
        _readHandler.SetResult(stored);
        _context.JSInterop.Setup<bool>("labirintTheme.isSystemDark").SetResult(expectedIsDark == false);

        var service = CreateService();
        await service.InitializeAsync();

        Assert.That(service.IsDark, Is.EqualTo(expectedIsDark));
    }

    /// <summary>
    /// Тестирует, что ThemeService.ToggleAsync переключает тему, сохраняет её в localStorage и применяет через JS.
    /// Проверяет, что после переключения с тёмной темы IsDark становится false, в хранилище пишется "false", применённая тема – "light", а тостов нет.
    /// </summary>
    [Test]
    public async Task ToggleAsyncPersistsAndAppliesTest()
    {
        _readHandler.SetResult("true");

        var service = CreateService();
        await service.InitializeAsync();
        await service.ToggleAsync();

        Assert.Multiple(() =>
        {
            Assert.That(service.IsDark, Is.False);
            Assert.That(GetStoredValue(), Is.EqualTo("false"));
            Assert.That(GetAppliedTheme(), Is.EqualTo("light"));
            Assert.That(GetToasts(), Is.Empty);
        });
    }

    /// <summary>
    /// Тестирует, что ThemeService.ToggleAsync переживает отказ localStorage.setItem, всё равно применяет новую тему к странице и сообщает игроку о сбое.
    /// Проверяет, что при исключении JSException из записи в хранилище IsDark и применённая тема всё равно меняются на светлую, а игрок видит тост-предупреждение «Не удалось запомнить тему».
    /// </summary>
    [Test]
    public async Task WriteFailureStillAppliesThemeTest()
    {
        _readHandler.SetResult("true");
        _context.JSInterop.SetupVoid("localStorage.setItem", StorageKey, "false").SetException(new JSException("Хранилище недоступно"));

        var service = CreateService();
        await service.InitializeAsync();
        await service.ToggleAsync();

        Assert.Multiple(() =>
        {
            Assert.That(service.IsDark, Is.False);
            Assert.That(GetStoredValue(), Is.EqualTo("false"));
            Assert.That(GetAppliedTheme(), Is.EqualTo("light"));
            Assert.That(GetToasts(), Is.EqualTo(new[] { ("Не удалось запомнить тему", UiSeverity.Warning) }));
        });
    }

    /// <summary>
    /// Тестирует, что ThemeService.InitializeAsync переживает отказ localStorage.getItem и откатывается к системной теме.
    /// Проверяет, что при исключении JSException из чтения хранилища IsDark и применённая тема совпадают с системным значением.
    /// </summary>
    [Test]
    public async Task ReadFailureFallsBackToSystemThemeTest()
    {
        _readHandler.SetException(new JSException("Хранилище недоступно"));
        _context.JSInterop.Setup<bool>("labirintTheme.isSystemDark").SetResult(false);

        var service = CreateService();
        await service.InitializeAsync();

        Assert.Multiple(() =>
        {
            Assert.That(service.IsDark, Is.False);
            Assert.That(GetAppliedTheme(), Is.EqualTo("light"));
        });
    }

    private ThemeService CreateService()
    {
        LocalStorageService localStorage = new(_context.JSInterop.JSRuntime);
        return new(_context.JSInterop.JSRuntime, localStorage, _toastService, NullLogger<ThemeService>.Instance);
    }

    private IEnumerable<(string Message, UiSeverity Severity)> GetToasts()
    {
        return _toastService.Toasts.Select(toast => (toast.Message, toast.Severity));
    }

    private string? GetAppliedTheme()
    {
        var invocations = _context.JSInterop.Invocations["labirintTheme.apply"];

        Assert.That(invocations, Is.Not.Empty, "Тема ни разу не применялась");

        return invocations.Last().Arguments[0] as string;
    }

    private string? GetStoredValue()
    {
        return _context.JSInterop.VerifyInvoke("localStorage.setItem").Arguments[1] as string;
    }
}
