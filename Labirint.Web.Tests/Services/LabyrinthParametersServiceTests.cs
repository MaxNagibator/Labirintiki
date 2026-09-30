using System.Globalization;
using Bunit;
using Labirint.Web.Parameters;
using Labirint.Web.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;

namespace Labirint.Web.Tests.Services;

[TestFixture]
public class LabyrinthParametersServiceTests
{
    private const string StorageKey = LabyrinthParameters.LocalStorageKey;

    private BunitContext _context = null!;
    private JSRuntimeInvocationHandler<string?> _readHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new();
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
    /// Тестирует, что LabyrinthParametersService.InitializeAsync разово мигрирует устаревший цвет по умолчанию на новый и сохраняет результат.
    /// Проверяет, что после инициализации Current.Color равен DefaultColor, а обратно в localStorage.setItem уходит JSON уже с новым цветом.
    /// </summary>
    [Test]
    public async Task LegacyColorIsMigratedAndPersistedTest()
    {
        _readHandler.SetResult(BuildJson(LabyrinthParameters.LegacyDefaultColor));

        var service = CreateService();
        await service.InitializeAsync();

        Assert.Multiple(() =>
        {
            Assert.That(service.Current.Color, Is.EqualTo(LabyrinthParameters.DefaultColor));
            Assert.That(GetStoredJson(), Does.Contain(LabyrinthParameters.DefaultColor));
        });
    }

    /// <summary>
    /// Тестирует, что LabyrinthParametersService.InitializeAsync не трогает пользовательский цвет, отличный от устаревшего значения по умолчанию.
    /// Проверяет, что Current.Color остаётся сохранённым значением и localStorage.setItem вовсе не вызывается.
    /// </summary>
    [Test]
    public async Task CustomColorIsNotRewrittenTest()
    {
        _readHandler.SetResult(BuildJson("#123456"));

        var service = CreateService();
        await service.InitializeAsync();

        Assert.Multiple(() =>
        {
            Assert.That(service.Current.Color, Is.EqualTo("#123456"));
            _context.JSInterop.VerifyNotInvoke("localStorage.setItem");
        });
    }

    /// <summary>
    /// Тестирует, что LabyrinthParametersService.SaveAsync обновляет Current, поднимает Changed и записывает параметры в localStorage дефолтной сериализацией JSON в PascalCase.
    /// Проверяет, что Current становится сохранённым экземпляром, событие Changed приходит один раз, а записанный JSON совпадает с PascalCase-представлением полей.
    /// </summary>
    [Test]
    public async Task SaveAsyncUpdatesCurrentAndRaisesChangedTest()
    {
        var service = CreateService();
        var changeCount = 0;

        service.Changed += (_, _) => changeCount++;

        LabyrinthParameters parameters = new()
        {
            Color = "#123456",
            IsSoundOn = false,
            SoundVolume = 0.5f,
        };

        await service.SaveAsync(parameters);

        Assert.Multiple(() =>
        {
            Assert.That(service.Current, Is.SameAs(parameters));
            Assert.That(changeCount, Is.EqualTo(1));
            Assert.That(GetStoredJson(), Is.EqualTo(BuildJson("#123456", false, 0.5f)));
        });
    }

    /// <summary>
    /// Тестирует, что LabyrinthParametersService.InitializeAsync переживает отказ localStorage при чтении и остаётся на параметрах по умолчанию.
    /// Проверяет, что при исключении JSException из localStorage.getItem Current сохраняет цвет, звук и громкость по умолчанию.
    /// </summary>
    [Test]
    public async Task ReadFailureKeepsDefaultsTest()
    {
        _readHandler.SetException(new JSException("Хранилище недоступно"));

        var service = CreateService();
        await service.InitializeAsync();

        Assert.Multiple(() =>
        {
            Assert.That(service.Current.Color, Is.EqualTo(LabyrinthParameters.DefaultColor));
            Assert.That(service.Current.IsSoundOn, Is.True);
            Assert.That(service.Current.SoundVolume, Is.EqualTo(1.0f));
        });
    }

    /// <summary>
    /// Тестирует, что сбой записи при миграции устаревшего цвета не отбрасывает уже прочитанные параметры.
    /// Проверяет, что при исключении JSException из localStorage.setItem Current получает новый цвет по умолчанию, а звук и громкость остаются прочитанными из хранилища.
    /// </summary>
    [Test]
    public async Task MigrationWriteFailureKeepsReadParametersTest()
    {
        _readHandler.SetResult(BuildJson(LabyrinthParameters.LegacyDefaultColor, false, 0.5f));
        _context.JSInterop.SetupVoid("localStorage.setItem", _ => true).SetException(new JSException("Хранилище переполнено"));

        var service = CreateService();
        await service.InitializeAsync();

        Assert.Multiple(() =>
        {
            Assert.That(service.Current.Color, Is.EqualTo(LabyrinthParameters.DefaultColor));
            Assert.That(service.Current.IsSoundOn, Is.False);
            Assert.That(service.Current.SoundVolume, Is.EqualTo(0.5f));
        });
    }

    private static string BuildJson(string color, bool isSoundOn = true, float soundVolume = 1.0f)
    {
        return $$"""{"Color":"{{color}}","IsSoundOn":{{(isSoundOn ? "true" : "false")}},"SoundVolume":{{soundVolume.ToString(CultureInfo.InvariantCulture)}}}""";
    }

    private LabyrinthParametersService CreateService()
    {
        LocalStorageService localStorage = new(_context.JSInterop.JSRuntime);
        return new(localStorage, NullLogger<LabyrinthParametersService>.Instance);
    }

    private string? GetStoredJson()
    {
        return _context.JSInterop.VerifyInvoke("localStorage.setItem").Arguments[1] as string;
    }
}
