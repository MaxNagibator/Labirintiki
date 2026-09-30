using Labirint.Web.Parameters;

namespace Labirint.Web.Tests.Session;

[TestFixture]
public class MazeSessionTests
{
    private const int MazeSize = 4;
    private const int NoWallsDensity = 0;

    private MazeSession _session = null!;

    [SetUp]
    public void SetUp()
    {
        _session = new(new SeedRandom(1));
    }

    [TearDown]
    public void TearDown()
    {
        _session.Dispose();
    }

    /// <summary>
    /// Тестирует, что MazeSession.GenerateAsync оставляет сессию в готовом к игре состоянии сразу после генерации.
    /// Проверяет, что IsReady истинен, Vision и Parameters заполнены, счётчики и флаги обнулены, а бегун стоит в клетке (0, 0).
    /// </summary>
    [Test]
    public async Task GenerateAsyncLeavesSessionReadyToPlayTest()
    {
        await _session.GenerateAsync(MazeSize, NoWallsDensity);

        Assert.Multiple(() =>
        {
            Assert.That(_session.IsReady, Is.True);
            Assert.That(_session.Vision, Is.Not.Null);
            Assert.That(_session.Parameters, Is.Not.Null);
            Assert.That(_session.MoveCount, Is.EqualTo(0));
            Assert.That(_session.IsExitFound, Is.False);
            Assert.That(_session.IsContinued, Is.False);
            Assert.That(_session.Runner.Position, Is.EqualTo(new Position(0, 0)));
            Assert.That(_session.Vision.Runner, Is.EqualTo(_session.Runner.Position));
        });
    }

    /// <summary>
    /// Тестирует, что MazeSession.GenerateAsync с переданным IProgress&lt;int&gt; сообщает о прогрессе генерации по возрастанию до завершения.
    /// Проверяет, что собранные значения прогресса не пусты, упорядочены и последнее из них равно 100.
    /// </summary>
    [Test]
    public async Task GenerateAsyncReportsProgressUpToCompletionTest()
    {
        const int width = 128;

        ProgressCollector progress = new();

        await _session.GenerateAsync(width, NoWallsDensity, progress);

        Assert.Multiple(() =>
        {
            Assert.That(progress.Values, Is.Not.Empty);
            Assert.That(progress.Values, Is.Ordered);
            Assert.That(progress.Values[^1], Is.EqualTo(100));
        });
    }

    /// <summary>
    /// Тестирует, что MazeSession отслеживает перемещения бегуна через подписку на Labyrinth, наращивая счётчик ходов и сдвигая Vision.
    /// Проверяет, что после двух успешных ходов MoveCount равен двум, а позиция бегуна в Vision и в самом Runner совпадает.
    /// </summary>
    [Test]
    public async Task MoveIncrementsMoveCountAndUpdatesVisionTest()
    {
        await _session.GenerateAsync(MazeSize, NoWallsDensity);

        _session.Labyrinth.Move(Direction.Right);
        _session.Labyrinth.Move(Direction.Bottom);

        Assert.Multiple(() =>
        {
            Assert.That(_session.MoveCount, Is.EqualTo(2));
            Assert.That(_session.Vision.Runner, Is.EqualTo(new Position(1, 1)));
            Assert.That(_session.Runner.Position, Is.EqualTo(new Position(1, 1)));
        });
    }

    /// <summary>
    /// Тестирует, что MazeSession не засчитывает ход, отклонённый движком из-за выхода за границу лабиринта.
    /// Проверяет, что попытка сдвинуться влево из стартовой клетки (0, 0) оставляет MoveCount равным нулю.
    /// </summary>
    [Test]
    public async Task MoveOutsideGridDoesNotIncrementMoveCountTest()
    {
        await _session.GenerateAsync(MazeSize, NoWallsDensity);

        _session.Labyrinth.Move(Direction.Left);

        Assert.That(_session.MoveCount, Is.EqualTo(0));
    }

    /// <summary>
    /// Тестирует, что MazeSession фиксирует достижение выхода и поднимает событие Finished ровно один раз.
    /// Проверяет, что после прохождения к выходу IsExitFound становится true и счётчик поднятых Finished равен единице.
    /// </summary>
    [Test]
    public async Task ReachingExitSetsFlagAndRaisesFinishedTest()
    {
        await _session.GenerateAsync(MazeSize, NoWallsDensity);

        var finishedRaisedCount = 0;
        _session.Finished += (_, _) => finishedRaisedCount++;

        MoveToExit();

        Assert.Multiple(() =>
        {
            Assert.That(_session.IsExitFound, Is.True);
            Assert.That(finishedRaisedCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Тестирует, что MazeSession.Continue снимает флаг найденного выхода и позволяет бегуну ходить дальше без повторных Finished.
    /// Проверяет, что после Continue IsContinued истинен, IsExitFound снят, а дальнейшие ходы не поднимают Finished повторно.
    /// </summary>
    [Test]
    public async Task ContinueClearsExitFoundAndSuppressesFurtherFinishedTest()
    {
        await _session.GenerateAsync(MazeSize, NoWallsDensity);

        var finishedRaisedCount = 0;
        _session.Finished += (_, _) => finishedRaisedCount++;

        MoveToExit();
        _session.Continue();

        _session.Labyrinth.Move(Direction.Top);
        _session.Labyrinth.Move(Direction.Bottom);

        Assert.Multiple(() =>
        {
            Assert.That(_session.IsContinued, Is.True);
            Assert.That(_session.IsExitFound, Is.False);
            Assert.That(finishedRaisedCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Тестирует, что MazeSession.GenerateAsync, вызванный повторно после найденного выхода, полностью сбрасывает состояние прежней партии.
    /// Проверяет, что после перегенерации MoveCount, IsExitFound и IsContinued возвращаются к начальным значениям, а бегун снова в клетке (0, 0).
    /// </summary>
    [Test]
    public async Task RegenerateAfterExitFoundResetsStateTest()
    {
        await _session.GenerateAsync(MazeSize, NoWallsDensity);

        MoveToExit();
        Assert.That(_session.IsExitFound, Is.True);

        await _session.GenerateAsync(MazeSize, NoWallsDensity);

        Assert.Multiple(() =>
        {
            Assert.That(_session.MoveCount, Is.EqualTo(0));
            Assert.That(_session.IsExitFound, Is.False);
            Assert.That(_session.IsContinued, Is.False);
            Assert.That(_session.Runner.Position, Is.EqualTo(new Position(0, 0)));
        });
    }

    private void MoveToExit()
    {
        _session.Labyrinth.Move(Direction.Right);
        _session.Labyrinth.Move(Direction.Right);
        _session.Labyrinth.Move(Direction.Bottom);
        _session.Labyrinth.Move(Direction.Bottom);
        _session.Labyrinth.Move(Direction.Bottom);
    }

    private sealed class ProgressCollector : IProgress<int>
    {
        public List<int> Values { get; } = [];

        public void Report(int value)
        {
            Values.Add(value);
        }
    }
}
