namespace Labirint.Web.Tests.Drawing;

[TestFixture]
public class VisionTests
{
    private const int MazeWidth = 16;
    private const int MazeHeight = 16;

    /// <summary>
    /// Тестирует, что Vision.GetDraw держит бегуна в геометрическом центре кадра независимо от его положения в лабиринте.
    /// Проверяет, что для угловых и центральной позиций координаты отрисовки бегуна равны Range по обеим осям.
    /// </summary>
    /// <param name="x">Позиция X бегуна</param>
    /// <param name="y">Позиция Y бегуна</param>
    [TestCase(0, 0)]
    [TestCase(15, 15)]
    [TestCase(7, 7)]
    [TestCase(0, 15)]
    public void RunnerStaysInFrameCenterTest(int x, int y)
    {
        Vision vision = new(MazeWidth, MazeHeight);

        vision.SetPosition((x, y));

        Assert.That(vision.GetDraw(vision.Runner), Is.EqualTo(new Position(vision.Range, vision.Range)));
    }

    /// <summary>
    /// Тестирует, что Vision.Origin не обрезается границами лабиринта, в отличие от Start и Finish.
    /// Проверяет, что у края лабиринта Origin уходит за пределы сетки в отрицательную или превышающую размер сторону.
    /// </summary>
    /// <param name="x">Позиция X бегуна</param>
    /// <param name="y">Позиция Y бегуна</param>
    /// <param name="expectedOriginX">Ожидаемая координата X начала кадра</param>
    /// <param name="expectedOriginY">Ожидаемая координата Y начала кадра</param>
    [TestCase(0, 0, -3, -3)]
    [TestCase(15, 15, 12, 12)]
    [TestCase(7, 7, 4, 4)]
    public void OriginIsNotClampedTest(int x, int y, int expectedOriginX, int expectedOriginY)
    {
        Vision vision = new(MazeWidth, MazeHeight);

        vision.SetPosition((x, y));

        Assert.That(vision.Origin, Is.EqualTo(new Position(expectedOriginX, expectedOriginY)));
    }

    /// <summary>
    /// Тестирует, что Vision.Start и Vision.Finish обрезаны границами лабиринта, в отличие от Origin.
    /// Проверяет, что у края и в углу лабиринта видимый диапазон клеток не выходит за пределы сетки.
    /// </summary>
    /// <param name="x">Позиция X бегуна</param>
    /// <param name="y">Позиция Y бегуна</param>
    /// <param name="expectedStartX">Ожидаемая координата X начала диапазона</param>
    /// <param name="expectedStartY">Ожидаемая координата Y начала диапазона</param>
    /// <param name="expectedFinishX">Ожидаемая координата X конца диапазона</param>
    /// <param name="expectedFinishY">Ожидаемая координата Y конца диапазона</param>
    [TestCase(0, 0, 0, 0, 3, 3)]
    [TestCase(15, 15, 12, 12, 15, 15)]
    [TestCase(7, 7, 4, 4, 10, 10)]
    [TestCase(0, 15, 0, 12, 3, 15)]
    public void StartAndFinishAreClampedToMazeTest(int x, int y, int expectedStartX, int expectedStartY, int expectedFinishX, int expectedFinishY)
    {
        Vision vision = new(MazeWidth, MazeHeight);

        vision.SetPosition((x, y));

        Assert.Multiple(() =>
        {
            Assert.That(vision.Start, Is.EqualTo(new Position(expectedStartX, expectedStartY)));
            Assert.That(vision.Finish, Is.EqualTo(new Position(expectedFinishX, expectedFinishY)));
        });
    }

    /// <summary>
    /// Тестирует, что Vision для лабиринта меньше радиуса видимости показывает его целиком.
    /// Проверяет, что Start и Finish совпадают с границами лабиринта 2x2, а координата отрисовки дальнего угла смещена самим лабиринтом, а не радиусом.
    /// </summary>
    [Test]
    public void SmallMazeIsFullyVisibleTest()
    {
        Vision vision = new(2, 2);

        vision.SetPosition((0, 0));

        Assert.Multiple(() =>
        {
            Assert.That(vision.Start, Is.EqualTo(new Position(0, 0)));
            Assert.That(vision.Finish, Is.EqualTo(new Position(1, 1)));
            Assert.That(vision.GetDraw((1, 1)), Is.EqualTo(new Position(4, 4)));
        });
    }
}
