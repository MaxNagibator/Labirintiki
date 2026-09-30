using Labirint.Web.Parameters;

namespace Labirint.Web.Tests.Drawing;

[TestFixture]
public class MazeRenderParametersTests
{
    /// <summary>
    /// Тестирует, что MazeRenderParameters.RenderRange считает размер кадра в пикселях по радиусу видимости, размеру клетки и толщине стены.
    /// Проверяет, что для разных сочетаний радиуса, размера клетки и толщины стены RenderRange совпадает с ожидаемым значением формулы.
    /// </summary>
    /// <param name="visionRange">Радиус видимости</param>
    /// <param name="boxSize">Размер клетки в пикселях</param>
    /// <param name="wallWidth">Толщина стены в пикселях</param>
    /// <param name="expectedRenderRange">Ожидаемый размер кадра</param>
    [TestCase(3, 64, 6, 454)]
    [TestCase(0, 64, 6, 70)]
    [TestCase(3, 32, 4, 228)]
    public void RenderRangeMatchesVisionAndBoxFormulaTest(int visionRange, int boxSize, int wallWidth, int expectedRenderRange)
    {
        Vision vision = new(16, 16, visionRange);
        vision.SetPosition((0, 0));

        MazeRenderParameters parameters = new(new(new SeedRandom(1)), boxSize, wallWidth, vision);

        Assert.That(parameters.RenderRange, Is.EqualTo(expectedRenderRange));
    }
}
