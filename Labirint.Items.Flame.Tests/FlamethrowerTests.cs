namespace Labirint.Items.Flame.Tests;

[TestFixture]
public class FlamethrowerTests
{
    private const int Size = 5;

    private Labyrinth _labyrinth = null!;
    private Flamethrower _flamethrower = null!;

    [SetUp]
    public void SetUp()
    {
        _labyrinth = new(new SeedRandom(1));
        _labyrinth.Init(Size, Size, 0, []);
        _flamethrower = new();
    }

    /// <summary>
    /// Тестирует, что огнемёт сносит несколько стен подряд в выбранном направлении.
    /// Проверяет, что три стены по направлению струи исчезают, а четвёртая остаётся целой.
    /// </summary>
    [Test]
    public void FlameBreaksWallsInRowTest()
    {
        for (var x = 0; x < 4; x++)
        {
            _labyrinth.CreateWall((x, 0), Direction.Right);
        }

        _flamethrower.Use((0, 0), Direction.Right, _labyrinth);

        Assert.Multiple(() =>
        {
            Assert.That(_labyrinth[(0, 0)].ContainsWall(Direction.Right), Is.False);
            Assert.That(_labyrinth[(1, 0)].ContainsWall(Direction.Right), Is.False);
            Assert.That(_labyrinth[(2, 0)].ContainsWall(Direction.Right), Is.False);
            Assert.That(_labyrinth[(3, 0)].ContainsWall(Direction.Right), Is.True);
        });
    }

    /// <summary>
    /// Тестирует, что струя огнемёта выдыхается у края лабиринта и не выходит за сетку.
    /// Проверяет, что стена последней клетки перед краем снята, а породу за краем струя не трогает.
    /// </summary>
    [Test]
    public void FlameStopsAtLabyrinthEdgeTest()
    {
        _labyrinth.CreateWall((Size - 2, 0), Direction.Right);

        Assert.DoesNotThrow(() => _flamethrower.Use((Size - 2, 0), Direction.Right, _labyrinth));

        Assert.Multiple(() =>
        {
            Assert.That(_labyrinth[(Size - 2, 0)].ContainsWall(Direction.Right), Is.False);
            Assert.That(_labyrinth[(Size - 1, 0)].ContainsWall(Direction.Right), Is.True);
        });
    }

    /// <summary>
    /// Тестирует, что огнемёт сам объявляет свой вид и свои характеристики для витрины.
    /// Проверяет, что вид предмета – ультимативное снаряжение, а среди характеристик есть дальность струи.
    /// </summary>
    [Test]
    public void FlamethrowerDescribesItselfTest()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_flamethrower.Kind, Is.EqualTo("Ультимативное снаряжение"));
            Assert.That(_flamethrower.Stats.Select(stat => stat.Label), Does.Contain("Дальность"));
        });
    }
}
