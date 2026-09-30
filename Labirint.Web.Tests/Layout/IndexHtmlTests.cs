using System.Text.RegularExpressions;

namespace Labirint.Web.Tests.Layout;

[TestFixture]
public class IndexHtmlTests
{
    private const string PageFileName = "index.html";
    private const string BlazorScript = "_framework/blazor.webassembly#[.{fingerprint}].js";

    /// <summary>
    /// Тестирует, что каждый скрипт приложения подключается с плейсхолдером отпечатка, который публикация заменяет хэшем содержимого.
    /// Проверяет, что у всех src вида scripts/*.js в index.html стоит #[.{fingerprint}] перед расширением.
    /// </summary>
    [Test]
    public void AppScriptsHaveFingerprintTest()
    {
        var sources = ReadScripts()
            .Select(script => script.Source)
            .Where(source => source.StartsWith("scripts/", StringComparison.Ordinal))
            .ToList();

        Assert.That(sources, Is.Not.Empty);
        Assert.That(sources, Has.All.Match(@"^scripts/\w+#\[\.\{fingerprint\}\]\.js$"));
    }

    /// <summary>
    /// Тестирует, что под политикой CSP с хэшами единственный встроенный скрипт страницы – карта импорта, которую заполняет публикация.
    /// Проверяет, что среди тегов script без src ровно один, и у него type="importmap".
    /// </summary>
    [Test]
    public void OnlyImportMapIsInlineTest()
    {
        var inline = ReadScripts().Where(script => script.Source == string.Empty).ToList();

        Assert.That(inline, Has.Count.EqualTo(1));
        Assert.That(inline[0].Attributes, Does.Contain("type=\"importmap\""));
    }

    /// <summary>
    /// Тестирует, что рантайм Blazor стартует только после скриптов приложения, даже если браузер грузит их по одному.
    /// Проверяет, что все скрипты в body помечены defer, а blazor.webassembly.js стоит среди них последним.
    /// </summary>
    [Test]
    public void BlazorStartsAfterAppScriptsTest()
    {
        var html = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, PageFileName));
        var body = html[html.IndexOf("<body>", StringComparison.Ordinal)..];
        var scripts = ParseScripts(body);

        Assert.That(scripts, Has.Count.GreaterThan(1));
        Assert.That(scripts.Select(script => Regex.IsMatch(script.Attributes, @"\sdefer\b")), Has.All.True);
        Assert.That(scripts[^1].Source, Is.EqualTo(BlazorScript));
    }

    private static List<(string Attributes, string Source)> ReadScripts()
    {
        return ParseScripts(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, PageFileName)));
    }

    private static List<(string Attributes, string Source)> ParseScripts(string html)
    {
        return Regex.Matches(html, "<script(?<attributes>[^>]*)>", RegexOptions.Singleline)
            .Select(match => match.Groups["attributes"].Value)
            .Select(attributes => (attributes, Regex.Match(attributes, "\\ssrc=\"(?<source>[^\"]*)\"").Groups["source"].Value))
            .ToList();
    }
}
