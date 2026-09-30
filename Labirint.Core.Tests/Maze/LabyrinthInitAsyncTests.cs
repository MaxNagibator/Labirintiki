namespace Labirint.Core.Tests.Maze;

[TestFixture]
public class LabyrinthInitAsyncTests
{
    private const int Seed = 7;

    /// <summary>
    /// Тестирует, что Labyrinth.InitAsync генерирует тот же лабиринт, что и синхронный Init при одинаковом зерне.
    /// Проверяет, что снимки стен и признака выхода по всем клеткам у асинхронно и синхронно сгенерированных лабиринтов совпадают.
    /// </summary>
    /// <param name="size">Ширина и высота лабиринта</param>
    /// <param name="density">Плотность стен</param>
    [TestCase(16, 40)]
    [TestCase(64, 60)]
    public async Task AsyncInitRepeatsSyncInitTest(int size, int density)
    {
        Labyrinth expected = new(new SeedRandom(Seed));
        Labyrinth actual = new(new SeedRandom(Seed));

        expected.Init(size, size, density);
        await actual.InitAsync(size, size, density);

        Assert.That(Snapshot(actual), Is.EqualTo(Snapshot(expected)));
    }

    /// <summary>
    /// Тестирует, что Labyrinth.InitAsync на большом лабиринте регулярно отчитывается о прогрессе через переданный IProgress.
    /// Проверяет, что значения прогресса идут по возрастанию, их больше одного и последнее равно 100.
    /// </summary>
    [Test]
    public async Task ProgressIsReportedUpToHundredTest()
    {
        Labyrinth labyrinth = new(new SeedRandom(Seed));
        ProgressCollector progress = new();

        await labyrinth.InitAsync(200, 200, 40, progress: progress);

        Assert.Multiple(() =>
        {
            Assert.That(progress.Values, Is.Ordered);
            Assert.That(progress.Values, Has.Count.GreaterThan(1));
            Assert.That(progress.Values.Last(), Is.EqualTo(100));
        });
    }

    /// <summary>
    /// Тестирует, что Labyrinth.InitAsync на маленьком лабиринте генерирует его одним проходом без промежуточной отдачи управления.
    /// Проверяет, что прогресс сообщается ровно один раз и сразу со значением 100.
    /// </summary>
    [Test]
    public async Task SmallMazeIsGeneratedInSinglePassTest()
    {
        Labyrinth labyrinth = new(new SeedRandom(Seed));
        ProgressCollector progress = new();

        await labyrinth.InitAsync(16, 16, 40, progress: progress);

        Assert.That(progress.Values, Is.EqualTo(new[] { 100 }));
    }

    /// <summary>
    /// Тестирует, что Labyrinth.InitAsync прекращает генерацию на ближайшей отдаче управления после отмены токена.
    /// Проверяет, что генерация, отменённая на первом отчёте о прогрессе, бросает OperationCanceledException и больше не сообщает прогресс.
    /// </summary>
    [Test]
    public void CancelledInitStopsGenerationTest()
    {
        Labyrinth labyrinth = new(new SeedRandom(Seed));
        using CancellationTokenSource cancellation = new();
        ProgressCollector progress = new(cancellation.Cancel);

        Assert.Multiple(() =>
        {
            Assert.ThrowsAsync<OperationCanceledException>(() => labyrinth.InitAsync(200, 200, 40, progress: progress, cancellationToken: cancellation.Token));
            Assert.That(progress.Values, Has.Count.EqualTo(1));
        });
    }

    private static (Direction Walls, bool IsExit)[] Snapshot(Labyrinth labyrinth)
    {
        var tiles = new (Direction, bool)[labyrinth.Width * labyrinth.Height];

        for (var x = 0; x < labyrinth.Width; x++)
        {
            for (var y = 0; y < labyrinth.Height; y++)
            {
                tiles[x * labyrinth.Height + y] = (labyrinth[x, y].Walls, labyrinth[x, y].IsExit);
            }
        }

        return tiles;
    }

    private sealed class ProgressCollector(Action? onReport = null) : IProgress<int>
    {
        public List<int> Values { get; } = [];

        public void Report(int value)
        {
            Values.Add(value);
            onReport?.Invoke();
        }
    }
}
