using Labirint.Web.Components;

namespace Labirint.Web.Tests.Components;

[TestFixture]
public class RandomGeneratorTests
{
    /// <summary>
    /// Тестирует, что RandomGenerator.ParseQueryNumber разбирает целое из параметра адреса так же, как его разбирал Blazor для int?.
    /// Проверяет, что целые, включая знак, пробелы и значения вне игрового диапазона, возвращаются числом.
    /// </summary>
    /// <param name="value">Значение параметра в адресе</param>
    /// <param name="expected">Ожидаемое число</param>
    [TestCase("16", 16)]
    [TestCase("-5", -5)]
    [TestCase("+7", 7)]
    [TestCase(" 40 ", 40)]
    [TestCase("100000", 100000)]
    public void IntegerIsParsedTest(string value, int expected)
    {
        Assert.That(RandomGenerator.ParseQueryNumber(value), Is.EqualTo(expected));
    }

    /// <summary>
    /// Тестирует, что RandomGenerator.ParseQueryNumber считает нечисловое или непредставимое в int значение отсутствующим, а не бросает.
    /// Проверяет, что для таких значений возвращается null.
    /// </summary>
    /// <param name="value">Значение параметра в адресе</param>
    [TestCase(null)]
    [TestCase("")]
    [TestCase("abc")]
    [TestCase("1.5")]
    [TestCase("1,5")]
    [TestCase("99999999999")]
    public void NonIntegerIsAbsentTest(string? value)
    {
        Assert.That(RandomGenerator.ParseQueryNumber(value), Is.Null);
    }
}
