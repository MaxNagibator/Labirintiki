using Labirint.Core;
using Labirint.Web.Common.Control;
using Labirint.Web.Common.Control.Schemes;

namespace Labirint.Web.Tests.Control;

[TestFixture]
public class KeyBindingMapTests
{
    /// <summary>
    /// Тестирует, что KeyBindingMap.FindDigit распознаёт коды клавиш цифрового ряда DigitN.
    /// Проверяет, что для кодов Digit0, Digit7 и Digit9 метод возвращает соответствующую цифру.
    /// </summary>
    /// <param name="code">Код клавиши</param>
    /// <param name="expectedDigit">Ожидаемая цифра</param>
    [TestCase("Digit0", 0)]
    [TestCase("Digit7", 7)]
    [TestCase("Digit9", 9)]
    public void DigitIsRecognizedTest(string code, int expectedDigit)
    {
        Assert.That(KeyBindingMap.FindDigit(code), Is.EqualTo(expectedDigit));
    }

    /// <summary>
    /// Тестирует, что KeyBindingMap.FindDigit отклоняет коды, не являющиеся кодом цифровой клавиши DigitN.
    /// Проверяет, что для усечённого кода, буквенного суффикса, кода Numpad, кода буквы и кода в нижнем регистре метод возвращает null.
    /// </summary>
    /// <param name="code">Код клавиши</param>
    [TestCase("Digit")]
    [TestCase("DigitA")]
    [TestCase("Numpad1")]
    [TestCase("KeyD")]
    [TestCase("digit1")]
    public void NotADigitReturnsNullTest(string code)
    {
        Assert.That(KeyBindingMap.FindDigit(code), Is.Null);
    }

    /// <summary>
    /// Тестирует, что KeyBindingMap.Rebuild строит привязку кодов клавиш к направлениям по схеме управления.
    /// Проверяет, что FindDirection для клавиш ClassicScheme возвращает соответствующие им направления Left, Top, Right, Bottom.
    /// </summary>
    [Test]
    public void DirectionsAreBoundToSchemeTest()
    {
        ClassicScheme scheme = new();
        KeyBindingMap map = new();

        map.Rebuild(scheme, null);

        Assert.Multiple(() =>
        {
            Assert.That(map.FindDirection(scheme.MoveLeft), Is.EqualTo(Direction.Left));
            Assert.That(map.FindDirection(scheme.MoveUp), Is.EqualTo(Direction.Top));
            Assert.That(map.FindDirection(scheme.MoveRight), Is.EqualTo(Direction.Right));
            Assert.That(map.FindDirection(scheme.MoveDown), Is.EqualTo(Direction.Bottom));
        });
    }

    /// <summary>
    /// Тестирует, что KeyBindingMap не находит привязку для кода клавиши, не входящего в схему управления.
    /// Проверяет, что FindDirection и FindItem для незнакомого кода "F13" возвращают null.
    /// </summary>
    [Test]
    public void UnknownCodeHasNoBindingTest()
    {
        KeyBindingMap map = new();

        map.Rebuild(new ClassicScheme(), null);

        Assert.Multiple(() =>
        {
            Assert.That(map.FindDirection("F13"), Is.Null);
            Assert.That(map.FindItem("F13"), Is.Null);
        });
    }

    /// <summary>
    /// Тестирует, что повторный KeyBindingMap.Rebuild с новой схемой полностью заменяет прежние привязки направлений.
    /// Проверяет, что коды клавиш прежней схемы, не встречающиеся в новой, перестают находить направление.
    /// </summary>
    [Test]
    public void SchemeChangeReplacesDirectionsTest()
    {
        ClassicScheme classic = new();
        AlternativeScheme alternative = new();
        KeyBindingMap map = new();

        map.Rebuild(classic, null);
        map.Rebuild(alternative, null);

        var replacedCodes = new[] { classic.MoveLeft, classic.MoveUp, classic.MoveRight, classic.MoveDown }
            .Except([alternative.MoveLeft, alternative.MoveUp, alternative.MoveRight, alternative.MoveDown]);

        Assert.That(replacedCodes.Select(key => map.FindDirection(key)), Has.All.Null);
    }

    /// <summary>
    /// Тестирует, что KeyBindingMap.Rebuild с переданным инвентарём привязывает предметы к их клавишам активации.
    /// Проверяет, что FindItem по клавише активации каждого предмета с ControlSettings возвращает именно этот предмет.
    /// </summary>
    [Test]
    public void ItemsAreBoundToActivateKeysTest()
    {
        Inventory inventory = new();
        ClassicScheme scheme = new();
        KeyBindingMap map = new();

        map.Rebuild(scheme, inventory);

        var boundItems = inventory.AllItems.Where(item => item.ControlSettings != null).ToList();

        Assert.That(boundItems, Is.Not.Empty);

        Assert.That(boundItems.Select(item => map.FindItem(scheme.GetActivateKey(item.ControlSettings!).KeyCode)),
            Is.EqualTo(boundItems));
    }
}
