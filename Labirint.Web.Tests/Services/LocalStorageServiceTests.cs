using Bunit;
using Labirint.Web.Parameters;
using Labirint.Web.Services;

namespace Labirint.Web.Tests.Services;

[TestFixture]
public class LocalStorageServiceTests
{
    private const string Key = "TestKey";

    private BunitContext _context = null!;
    private LocalStorageService _localStorage = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new();
        _context.JSInterop.Mode = JSRuntimeMode.Loose;

        _localStorage = new(_context.JSInterop.JSRuntime);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Тестирует, что LocalStorageService.GetItemAsync возвращает значение по умолчанию, когда в localStorage ничего осмысленного не лежит.
    /// Проверяет, что для отсутствующего, пустого и состоящего из пробелов значения строковый и nullable-логический результат равны null.
    /// </summary>
    /// <param name="stored">Значение, возвращаемое localStorage.getItem</param>
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public async Task MissingValueReturnsDefaultTest(string? stored)
    {
        _context.JSInterop.Setup<string?>("localStorage.getItem", Key).SetResult(stored);

        string? text = await _localStorage.GetItemAsync<string>(Key);
        bool? flag = await _localStorage.GetItemAsync<bool?>(Key);

        Assert.Multiple(() =>
        {
            Assert.That(text, Is.Null);
            Assert.That(flag, Is.Null);
        });
    }

    /// <summary>
    /// Тестирует, что LocalStorageService.GetItemAsync&lt;string&gt; читает и сырую строку, и строку в JSON-кавычках одинаково.
    /// Проверяет, что оба варианта хранения дают на выходе один и тот же текст без кавычек.
    /// </summary>
    /// <param name="stored">Значение, возвращаемое localStorage.getItem</param>
    /// <param name="expected">Ожидаемое прочитанное значение</param>
    [TestCase("Классическая схема (←↕→)", "Классическая схема (←↕→)")]
    [TestCase("\"Классическая схема (←↕→)\"", "Классическая схема (←↕→)")]
    public async Task RawAndQuotedStringsReadBackTest(string stored, string expected)
    {
        _context.JSInterop.Setup<string?>("localStorage.getItem", Key).SetResult(stored);

        string? value = await _localStorage.GetItemAsync<string>(Key);

        Assert.That(value, Is.EqualTo(expected));
    }

    /// <summary>
    /// Тестирует, что LocalStorageService.GetItemAsync десериализует уже лежащий у игроков JSON в PascalCase.
    /// Проверяет, что поля Color, IsSoundOn и SoundVolume старого формата корректно попадают в LabyrinthParameters.
    /// </summary>
    [Test]
    public async Task LegacyPascalCaseJsonIsReadBackTest()
    {
        const string legacyJson = """{"Color":"#123456","IsSoundOn":false,"SoundVolume":0.25}""";

        _context.JSInterop.Setup<string?>("localStorage.getItem", Key).SetResult(legacyJson);

        LabyrinthParameters? loaded = await _localStorage.GetItemAsync<LabyrinthParameters>(Key);

        Assert.Multiple(() =>
        {
            Assert.That(loaded?.Color, Is.EqualTo("#123456"));
            Assert.That(loaded?.IsSoundOn, Is.False);
            Assert.That(loaded?.SoundVolume, Is.EqualTo(0.25f));
        });
    }

    /// <summary>
    /// Тестирует, что LocalStorageService.SetItemAsync сериализует объект дефолтными JsonSerializerOptions, сохраняя PascalCase имён свойств.
    /// Проверяет, что записанный в localStorage.setItem JSON совпадает с ожидаемой PascalCase-строкой, без перехода на camelCase.
    /// </summary>
    [Test]
    public async Task StoredJsonKeepsPascalCaseTest()
    {
        LabyrinthParameters parameters = new()
        {
            Color = "#123456",
            IsSoundOn = false,
            SoundVolume = 0.5f,
        };

        await _localStorage.SetItemAsync(Key, parameters);

        Assert.That(GetStoredJson(), Is.EqualTo("""{"Color":"#123456","IsSoundOn":false,"SoundVolume":0.5}"""));
    }

    private string? GetStoredJson()
    {
        return _context.JSInterop.VerifyInvoke("localStorage.setItem").Arguments[1] as string;
    }
}
