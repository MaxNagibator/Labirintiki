using System.Text.Json;
using Bunit;
using Labirint.Web.Common.Control.Schemes;
using Labirint.Web.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;

namespace Labirint.Web.Tests.Services;

[TestFixture]
public class ControlSchemeServiceTests
{
    private const string StorageKey = nameof(ControlSchemeService);

    private static readonly TimeSpan SchemeChangeTimeout = TimeSpan.FromMilliseconds(500);

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
    /// Тестирует, что ControlSchemeService применяет схему управления, сохранённую в localStorage, ещё до первого обращения к ней.
    /// Проверяет, что CurrentScheme сразу после создания сервиса имеет тип AlternativeScheme, когда в хранилище лежит её имя.
    /// </summary>
    [Test]
    public void StoredSchemeIsAppliedTest()
    {
        _readHandler.SetResult(Quote(new AlternativeScheme().Name));

        var service = CreateService();

        Assert.That(service.CurrentScheme, Is.InstanceOf<AlternativeScheme>());
    }

    /// <summary>
    /// Тестирует, что ControlSchemeService поднимает ControlSchemeChanged, когда схема из localStorage подгружается асинхронно после конструктора.
    /// Проверяет, что событие приходит с той же схемой, что и CurrentScheme после завершения загрузки.
    /// </summary>
    [Test]
    public async Task StoredSchemeRaisesChangedTest()
    {
        var service = CreateService();
        var stored = service.AvailableSchemes.OfType<AlternativeScheme>().Single();

        var applied = await LoadAndWaitAsync(service, stored.Name);

        Assert.Multiple(() =>
        {
            Assert.That(applied, Is.SameAs(stored));
            Assert.That(service.CurrentScheme, Is.SameAs(stored));
        });
    }

    /// <summary>
    /// Тестирует, что ControlSchemeService игнорирует значение из localStorage, если оно не соответствует ни одной известной схеме.
    /// Проверяет, что для незнакомого имени и для пустой строки событие ControlSchemeChanged не приходит, а CurrentScheme остаётся DefaultScheme.
    /// </summary>
    /// <param name="storedName">Имя схемы, прочитанное из хранилища</param>
    [TestCase("Схемы с таким именем нет")]
    [TestCase("")]
    public async Task UnknownStoredSchemeKeepsDefaultTest(string storedName)
    {
        var service = CreateService();

        var applied = await LoadAndWaitAsync(service, storedName);

        Assert.Multiple(() =>
        {
            Assert.That(applied, Is.Null);
            Assert.That(service.CurrentScheme, Is.SameAs(service.DefaultScheme));
        });
    }

    /// <summary>
    /// Тестирует, что ControlSchemeService не перезаписывает выбор пользователя, если чтение localStorage завершается позже, чем пользователь сменил схему.
    /// Проверяет, что после выбора схемы вручную запоздавший результат чтения хранилища не поднимает ControlSchemeChanged и не меняет CurrentScheme.
    /// </summary>
    [Test]
    public async Task LateLoadDoesNotOverrideUserChoiceTest()
    {
        var service = CreateService();
        var chosen = service.AvailableSchemes.OfType<AlternativeScheme>().Single();

        await service.SetSchemeAsync(chosen);

        var applied = await LoadAndWaitAsync(service, service.DefaultScheme.Name);

        Assert.Multiple(() =>
        {
            Assert.That(applied, Is.Null);
            Assert.That(service.CurrentScheme, Is.SameAs(chosen));
        });
    }

    /// <summary>
    /// Тестирует, что ControlSchemeService переживает отказ localStorage при чтении и остаётся на значении по умолчанию.
    /// Проверяет, что при исключении JSException из localStorage.getItem CurrentScheme равен DefaultScheme.
    /// </summary>
    [Test]
    public void ReadFailureKeepsDefaultSchemeTest()
    {
        _readHandler.SetException(new JSException("Хранилище недоступно"));

        var service = CreateService();

        Assert.That(service.CurrentScheme, Is.SameAs(service.DefaultScheme));
    }

    /// <summary>
    /// Тестирует, что ControlSchemeService.SetSchemeAsync отвергает экземпляр схемы, не входящий в AvailableSchemes.
    /// Проверяет, что передача нового экземпляра AlternativeScheme, не зарегистрированного в сервисе, бросает ArgumentException.
    /// </summary>
    [Test]
    public void UnregisteredSchemeThrowsTest()
    {
        var service = CreateService();

        Assert.ThrowsAsync<ArgumentException>(() => service.SetSchemeAsync(new AlternativeScheme()));
    }

    /// <summary>
    /// Тестирует, что ControlSchemeService.SetSchemeAsync сохраняет выбранную схему в localStorage и применяет её.
    /// Проверяет, что localStorage.setItem получил имя схемы, CurrentScheme сменился на неё, а ControlSchemeChanged пришёл с ней же.
    /// </summary>
    [Test]
    public async Task ChosenSchemeIsSavedAndAppliedTest()
    {
        var service = CreateService();
        var chosen = service.AvailableSchemes.OfType<AlternativeScheme>().Single();
        IControlScheme? raised = null;
        service.ControlSchemeChanged += (_, scheme) => raised = scheme;

        await service.SetSchemeAsync(chosen);

        var write = _context.JSInterop.VerifyInvoke("localStorage.setItem");

        Assert.Multiple(() =>
        {
            Assert.That(write.Arguments, Is.EqualTo(new object[] { StorageKey, JsonSerializer.Serialize(chosen.Name) }));
            Assert.That(service.CurrentScheme, Is.SameAs(chosen));
            Assert.That(raised, Is.SameAs(chosen));
        });
    }

    /// <summary>
    /// Тестирует, что сбой записи в localStorage при смене схемы доходит до вызывающего, а не теряется молча.
    /// Проверяет, что SetSchemeAsync бросает исключение хранилища, CurrentScheme остаётся DefaultScheme, ControlSchemeChanged не приходит, а запоздавшее чтение хранилища после сбоя по-прежнему применяет сохранённую схему.
    /// </summary>
    [Test]
    public async Task SaveFailureIsRethrownAndSchemeKeptTest()
    {
        var service = CreateService();
        var chosen = service.AvailableSchemes.OfType<AlternativeScheme>().Single();
        IControlScheme? raised = null;
        service.ControlSchemeChanged += (_, scheme) => raised = scheme;

        _context.JSInterop.SetupVoid("localStorage.setItem", _ => true)
            .SetException(new JSException("Хранилище переполнено"));

        Assert.ThrowsAsync<JSException>(() => service.SetSchemeAsync(chosen));

        var schemeAfterFailure = service.CurrentScheme;
        var raisedAfterFailure = raised;
        var applied = await LoadAndWaitAsync(service, chosen.Name);

        Assert.Multiple(() =>
        {
            Assert.That(schemeAfterFailure, Is.SameAs(service.DefaultScheme));
            Assert.That(raisedAfterFailure, Is.Null);
            Assert.That(applied, Is.SameAs(chosen));
        });
    }

    private static string Quote(string value)
    {
        return $"\"{value}\"";
    }

    private async Task<IControlScheme?> LoadAndWaitAsync(ControlSchemeService service, string storedName)
    {
        TaskCompletionSource<IControlScheme> changed = new(TaskCreationOptions.RunContinuationsAsynchronously);

        service.ControlSchemeChanged += (_, scheme) => changed.TrySetResult(scheme);

        _readHandler.SetResult(Quote(storedName));

        var completed = await Task.WhenAny(changed.Task, Task.Delay(SchemeChangeTimeout));

        return completed == changed.Task ? await changed.Task : null;
    }

    private ControlSchemeService CreateService()
    {
        LocalStorageService localStorage = new(_context.JSInterop.JSRuntime);
        return new(localStorage, NullLogger<ControlSchemeService>.Instance);
    }
}
