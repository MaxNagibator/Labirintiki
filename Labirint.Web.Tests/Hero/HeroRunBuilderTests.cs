using System.Text.RegularExpressions;
using Labirint.Web.Common.Hero;

namespace Labirint.Web.Tests.Hero;

[TestFixture]
public class HeroRunBuilderTests
{
    private const int Cell = 24;
    private const int WallDrawStep = 9;
    private const int TrailStartPause = 400;
    private const int StepDuration = 45;
    private const int MinTrailDuration = 2200;
    private const int MaxTrailDuration = 7000;
    private const int SolvableAttempts = 12;

    /// <summary>
    /// Тестирует, что HeroRunBuilder.Build детерминирован для одного и того же зерна.
    /// Проверяет, что два построения с зерном 1 дают одинаковые Seed, TrailPath, Walls, IsSolved и Exit.
    /// </summary>
    [Test]
    public void BuildIsDeterministicForSameSeedTest()
    {
        var first = HeroRunBuilder.Build(1);
        var second = HeroRunBuilder.Build(1);

        Assert.Multiple(() =>
        {
            Assert.That(second.Seed, Is.EqualTo(first.Seed));
            Assert.That(second.TrailPath, Is.EqualTo(first.TrailPath));
            Assert.That(second.Walls, Is.EqualTo(first.Walls));
            Assert.That(second.IsSolved, Is.EqualTo(first.IsSolved));
            Assert.That(second.Exit, Is.EqualTo(first.Exit));
        });
    }

    /// <summary>
    /// Тестирует, что HeroRunBuilder.Build строит TrailPath как непрерывную цепочку соседних клеток.
    /// Проверяет, что каждая пара последовательных точек пути отличается ровно на размер клетки Cell по одной из осей.
    /// </summary>
    [Test]
    public void RouteIsContinuousChainOfAdjacentCellsTest()
    {
        var run = HeroRunBuilder.Build(1);
        var points = ParseTrailPoints(run.TrailPath);

        Assert.That(points, Has.Count.GreaterThan(1));

        for (var index = 1; index < points.Count; index++)
        {
            var (previousX, previousY) = points[index - 1];
            var (currentX, currentY) = points[index];

            var dx = Math.Abs(currentX - previousX);
            var dy = Math.Abs(currentY - previousY);

            Assert.That((dx == Cell && dy == 0) || (dx == 0 && dy == Cell), Is.True,
                $"Точки {index - 1} и {index} не смежны: ({previousX},{previousY}) -> ({currentX},{currentY})");
        }
    }

    /// <summary>
    /// Тестирует, что маршрут HeroRunBuilder.Build – это полный обход в глубину с тупиками и возвратами, а не кратчайший путь.
    /// Проверяет, что путь содержит повторяющиеся точки, то есть число уникальных точек меньше общего числа точек.
    /// </summary>
    [Test]
    public void RouteRevisitsDeadEndsWithBacktracksTest()
    {
        var run = HeroRunBuilder.Build(1);
        var points = ParseTrailPoints(run.TrailPath);

        Assert.That(points.Distinct().Count(), Is.LessThan(points.Count));
    }

    /// <summary>
    /// Тестирует, что HeroRunBuilder.Build при непроходимом лабиринте перебирает зёрна вперёд до первого проходимого, а не дальше двенадцати попыток.
    /// Проверяет, что итоговый Seed проходим, лежит в пределах SolvableAttempts от запрошенного, и что все промежуточные зёрна доводят до того же результата.
    /// </summary>
    /// <param name="requestedSeed">Исходное зерно</param>
    [TestCase(3)]
    [TestCase(17)]
    [TestCase(1042)]
    public void SeedSearchAdvancesToFirstSolvableMazeTest(int requestedSeed)
    {
        var run = HeroRunBuilder.Build(requestedSeed);

        Assert.Multiple(() =>
        {
            Assert.That(run.IsSolved, Is.True);
            Assert.That(run.Seed, Is.InRange(requestedSeed, requestedSeed + SolvableAttempts - 1));
        });

        for (var seed = requestedSeed; seed < run.Seed; seed++)
        {
            Assert.That(HeroRunBuilder.Build(seed).Seed, Is.EqualTo(run.Seed), $"Зерно {seed} должно доводить до того же проходимого лабиринта");
        }
    }

    /// <summary>
    /// Тестирует, что HeroRunBuilder.Build выводит длину и длительность анимации пути из числа шагов маршрута.
    /// Проверяет, что TrailLength равен числу шагов, умноженному на Cell, TrailDuration – ограниченному сверху и снизу произведению шагов на StepDuration, а Duration – сумме TrailDelay и TrailDuration.
    /// </summary>
    /// <param name="seed">Зерно генерации</param>
    [TestCase(1)]
    [TestCase(13)]
    public void WallsAndTimingsAreConsistentWithRouteLengthTest(int seed)
    {
        var run = HeroRunBuilder.Build(seed);
        var points = ParseTrailPoints(run.TrailPath);
        var steps = points.Count - 1;

        Assert.Multiple(() =>
        {
            Assert.That(run.Walls, Is.Not.Empty);
            Assert.That(run.TrailLength, Is.EqualTo(steps * Cell));
            Assert.That(run.TrailDuration, Is.EqualTo(Math.Clamp(steps * StepDuration, MinTrailDuration, MaxTrailDuration)));
            Assert.That(run.Duration, Is.EqualTo(run.TrailDelay + run.TrailDuration));
        });
    }

    /// <summary>
    /// Тестирует, что HeroRunBuilder.Build назначает стенам возрастающую задержку отрисовки с фиксированным шагом.
    /// Проверяет, что задержка каждой стены равна её индексу, умноженному на WallDrawStep, а TrailDelay равен задержке последней стены плюс TrailStartPause.
    /// </summary>
    [Test]
    public void WallDelaysIncreaseByFixedStepTest()
    {
        var run = HeroRunBuilder.Build(1);

        for (var index = 0; index < run.Walls.Count; index++)
        {
            Assert.That(run.Walls[index].Delay, Is.EqualTo(index * WallDrawStep));
        }

        Assert.That(run.TrailDelay, Is.EqualTo(run.Walls.Count * WallDrawStep + TrailStartPause));
    }

    private static List<(int X, int Y)> ParseTrailPoints(string trailPath)
    {
        var numbers = Regex.Matches(trailPath, @"-?\d+")
            .Select(match => int.Parse(match.Value))
            .ToArray();

        List<(int X, int Y)> points = [];

        for (var index = 0; index < numbers.Length; index += 2)
        {
            points.Add((numbers[index], numbers[index + 1]));
        }

        return points;
    }
}
