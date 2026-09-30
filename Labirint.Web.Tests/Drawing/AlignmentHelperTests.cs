using Labirint.Core.TileFeatures.Common;

namespace Labirint.Web.Tests.Drawing;

[TestFixture]
public class AlignmentHelperTests
{
    private const int BoxSize = 64;
    private const int WallWidth = 6;
    private const double Scale = 0.9;

    /// <summary>
    /// Тестирует, что AlignmentHelper.GetAlignmentParameters сдвигает позицию отрисовки сущности по девяти точкам выравнивания при масштабе меньше единицы.
    /// Проверяет, что для каждого значения Alignment итоговые координаты и размер уменьшенной сущности совпадают с ожидаемыми.
    /// </summary>
    /// <param name="alignment">Точка выравнивания</param>
    /// <param name="expectedX">Ожидаемая координата X</param>
    /// <param name="expectedY">Ожидаемая координата Y</param>
    [TestCase(Alignment.TopLeft, 70, 70)]
    [TestCase(Alignment.TopCenter, 73, 70)]
    [TestCase(Alignment.TopRight, 76, 70)]
    [TestCase(Alignment.CenterLeft, 70, 73)]
    [TestCase(Alignment.Center, 73, 73)]
    [TestCase(Alignment.CenterRight, 76, 73)]
    [TestCase(Alignment.BottomLeft, 70, 76)]
    [TestCase(Alignment.BottomCenter, 73, 76)]
    [TestCase(Alignment.BottomRight, 76, 76)]
    public void ScaledEntityIsShiftedByAlignmentTest(Alignment alignment, int expectedX, int expectedY)
    {
        var (entitySize, drawPosition) = AlignmentHelper.GetAlignmentParameters(BoxSize, WallWidth, Scale, (1, 1), alignment);

        Assert.Multiple(() =>
        {
            Assert.That(entitySize, Is.EqualTo(52));
            Assert.That(drawPosition, Is.EqualTo(new Position(expectedX, expectedY)));
        });
    }

    /// <summary>
    /// Тестирует, что AlignmentHelper.GetAlignmentParameters с выравниванием Stretch не учитывает толщину стены при расчёте размера и позиции.
    /// Проверяет, что итоговый размер и координаты отличаются от результата обычных выравниваний, где толщина стены вычитается дважды.
    /// </summary>
    [Test]
    public void StretchIgnoresWallWidthTest()
    {
        var (entitySize, drawPosition) = AlignmentHelper.GetAlignmentParameters(BoxSize, WallWidth, Scale, (1, 1), Alignment.Stretch);

        Assert.Multiple(() =>
        {
            Assert.That(entitySize, Is.EqualTo(57));
            Assert.That(drawPosition, Is.EqualTo(new Position(73, 73)));
        });
    }

    /// <summary>
    /// Тестирует, что AlignmentHelper.GetAlignmentParameters при масштабе 1 ставит сущность в угол клетки независимо от выбранного выравнивания.
    /// Проверяет, что размер равен BoxSize за вычетом WallWidth, а позиция отрисовки совпадает с левым верхним углом клетки со сдвигом на толщину стены.
    /// </summary>
    /// <param name="alignment">Точка выравнивания</param>
    [TestCase(Alignment.TopLeft)]
    [TestCase(Alignment.Center)]
    [TestCase(Alignment.BottomRight)]
    public void FullScaleKeepsCellCornerTest(Alignment alignment)
    {
        var (entitySize, drawPosition) = AlignmentHelper.GetAlignmentParameters(BoxSize, WallWidth, 1, (2, 3), alignment);

        Assert.Multiple(() =>
        {
            Assert.That(entitySize, Is.EqualTo(BoxSize - WallWidth));
            Assert.That(drawPosition, Is.EqualTo(new Position(2 * BoxSize + WallWidth, 3 * BoxSize + WallWidth)));
        });
    }

    /// <summary>
    /// Тестирует, что AlignmentHelper.GetAlignmentParameters отвергает значение Alignment, не входящее в перечисление.
    /// Проверяет, что вызов с числовым значением 42, приведённым к Alignment, бросает ArgumentOutOfRangeException.
    /// </summary>
    [Test]
    public void UnknownAlignmentThrowsTest()
    {
        Assert.That(() => AlignmentHelper.GetAlignmentParameters(BoxSize, WallWidth, Scale, (0, 0), (Alignment)42),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }
}
