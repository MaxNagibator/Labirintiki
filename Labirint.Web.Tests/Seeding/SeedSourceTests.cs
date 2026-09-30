using Labirint.Web.Common.Seeding;

namespace Labirint.Web.Tests.Seeding;

[TestFixture]
public class SeedSourceTests
{
    /// <summary>
    /// Тестирует, что SeedSource.GenerateSeed переводит текстовое зерно в детерминированное числовое хэшированием SHA256.
    /// Проверяет, что для конкретных текстовых зёрен результат совпадает с заранее известным числом.
    /// </summary>
    /// <param name="userSeed">Введённое пользователем зерно</param>
    /// <param name="expectedSeed">Ожидаемое числовое зерно</param>
    [TestCase("test", 2117040481)]
    [TestCase("лабиринт", 1551532478)]
    public void TextSeedIsHashedTest(string userSeed, int expectedSeed)
    {
        Assert.That(SeedSource.GenerateSeed(userSeed), Is.EqualTo(expectedSeed));
    }

    /// <summary>
    /// Тестирует, что SeedSource.GenerateSeed никогда не возвращает отрицательное число, включая крайний случай хэша int.MinValue.
    /// Проверяет, что для разных текстовых зёрен, включая пустую строку, результат неотрицателен.
    /// </summary>
    /// <param name="userSeed">Введённое пользователем зерно</param>
    [TestCase("")]
    [TestCase("test")]
    [TestCase("Zj9V4CPCUYKAg")]
    [TestCase("очень длинное зерно из нескольких слов")]
    public void GeneratedSeedIsNotNegativeTest(string userSeed)
    {
        Assert.That(SeedSource.GenerateSeed(userSeed), Is.GreaterThanOrEqualTo(0));
    }

    /// <summary>
    /// Тестирует, что SeedSource.Reload для чисто числового UserSeed использует значение как есть, минуя хэширование.
    /// Проверяет, что CurrentSeed после Reload равен числу, введённому пользователем.
    /// </summary>
    [Test]
    public void NumericSeedIsUsedAsIsTest()
    {
        SeedSource source = new()
        {
            UserSeed = "12345",
        };

        source.Reload();

        Assert.That(source.CurrentSeed, Is.EqualTo(12345));
    }

    /// <summary>
    /// Тестирует, что SeedSource.Reload для числовой строки, не помещающейся в int, откатывается к хэшированию как к текстовому зерну.
    /// Проверяет, что CurrentSeed для переполняющего числа совпадает с результатом SeedSource.GenerateSeed той же строки.
    /// </summary>
    [Test]
    public void TooLongNumericSeedFallsBackToHashTest()
    {
        const string userSeed = "99999999999999999999";

        SeedSource source = new()
        {
            UserSeed = userSeed,
        };

        source.Reload();

        Assert.That(source.CurrentSeed, Is.EqualTo(SeedSource.GenerateSeed(userSeed)));
    }

    /// <summary>
    /// Тестирует, что SeedSource.Reload(true) сбрасывает пользовательское зерно и генерирует новое случайное.
    /// Проверяет, что после принудительной перезагрузки UserSeed становится null, а CurrentSeed отличается от прежнего значения.
    /// </summary>
    [Test]
    public void ForcedReloadDropsUserSeedTest()
    {
        SeedSource source = new()
        {
            UserSeed = "12345",
        };

        source.Reload(true);

        Assert.Multiple(() =>
        {
            Assert.That(source.UserSeed, Is.Null);
            Assert.That(source.CurrentSeed, Is.Not.EqualTo(12345));
        });
    }

    /// <summary>
    /// Тестирует, что SeedSource.Repeat заново инициализирует генератор тем же зерном, повторяя случайную последовательность.
    /// Проверяет, что после Repeat первые десять чисел генератора совпадают с числами, полученными до вызова.
    /// </summary>
    [Test]
    public void SameSeedGivesSameSequenceTest()
    {
        SeedSource source = new()
        {
            UserSeed = "лабиринт",
        };

        source.Reload();
        var first = Enumerable.Range(0, 10).Select(_ => source.Generator.Next(1000)).ToArray();

        source.Repeat();
        var second = Enumerable.Range(0, 10).Select(_ => source.Generator.Next(1000)).ToArray();

        Assert.That(second, Is.EqualTo(first));
    }

    /// <summary>
    /// Тестирует, что SeedSource.ResetSeed помечает источник как требующий новой генерации.
    /// Проверяет, что после сброса зерна IsGenerateRequired становится true.
    /// </summary>
    [Test]
    public void ResetSeedRequiresGenerationTest()
    {
        SeedSource source = new()
        {
            UserSeed = "12345",
        };

        source.Reload();
        source.ResetSeed();

        Assert.That(source.IsGenerateRequired, Is.True);
    }

    /// <summary>
    /// Тестирует, что SeedSource.Repeat после ResetSeed возвращает источник к прежнему пользовательскому зерну.
    /// Проверяет, что после сброса и повтора IsGenerateRequired снова false, а CurrentSeed равен исходному пользовательскому зерну.
    /// </summary>
    [Test]
    public void RepeatAfterResetAppliesUserSeedAgainTest()
    {
        SeedSource source = new()
        {
            UserSeed = "12345",
        };

        source.Reload();
        source.ResetSeed();
        source.Repeat();

        Assert.Multiple(() =>
        {
            Assert.That(source.IsGenerateRequired, Is.False);
            Assert.That(source.CurrentSeed, Is.EqualTo(12345));
        });
    }

    /// <summary>
    /// Тестирует, что SeedSource.Repeat без пользовательского зерна не требует его наличия и порождает новое случайное зерно.
    /// Проверяет, что после сброса и повтора без UserSeed IsGenerateRequired становится false, а UserSeed остаётся null.
    /// </summary>
    [Test]
    public void RepeatAfterResetWithoutUserSeedGeneratesNewSeedTest()
    {
        SeedSource source = new();

        source.ResetSeed();
        source.Repeat();

        Assert.Multiple(() =>
        {
            Assert.That(source.IsGenerateRequired, Is.False);
            Assert.That(source.UserSeed, Is.Null);
        });
    }
}
