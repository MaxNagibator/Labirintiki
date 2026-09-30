using System.Text;

namespace Labirint.Web.Common.Hero;

public static class HeroRunBuilder
{
    private const int Size = 13;
    private const int Cell = 24;
    private const int Side = Size * Cell;
    private const int Density = 42;
    private const int ExitX = Size / 2;
    private const int ExitY = Size - 1;
    private const int ExitInset = 5;

    private const int WallDrawStep = 9;
    private const int TrailStartPause = 400;
    private const int StepDuration = 45;
    private const int MinTrailDuration = 2200;
    private const int MaxTrailDuration = 7000;
    private const int TailLength = 5 * Cell;

    private const int SolvableAttempts = 12;
    private const int SeedFloor = 10_000_000;
    private const int SeedCeiling = 100_000_000;

    public static HeroRun Build(int seed)
    {
        HeroRun? unsolved = null;

        for (var attempt = 0; attempt < SolvableAttempts; attempt++)
        {
            var run = Compose(seed + attempt);

            if (run.IsSolved)
            {
                return run;
            }

            unsolved ??= run;
        }

        return unsolved!;
    }

    public static HeroRun BuildNext()
    {
        return Build(Random.Shared.Next(SeedFloor, SeedCeiling));
    }

    private static HeroRun Compose(int seed)
    {
        Labyrinth labyrinth = new(new SeedRandom(seed));
        labyrinth.Init(Size, Size, Density, []);

        var walls = CollectWalls(labyrinth);
        var walk = Walk(labyrinth, seed, out var isSolved);

        var steps = walk.Count - 1;

        return new HeroRun(
            seed,
            Side,
            new HeroExit(ExitX * Cell + ExitInset, ExitY * Cell + ExitInset, Cell - 2 * ExitInset),
            walls,
            BuildTrailPath(walk),
            steps * Cell,
            TailLength,
            walls.Count * WallDrawStep + TrailStartPause,
            Math.Clamp(steps * StepDuration, MinTrailDuration, MaxTrailDuration),
            isSolved);
    }

    private static List<HeroWall> CollectWalls(Labyrinth labyrinth)
    {
        List<HeroWall> walls = [];

        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                var tile = labyrinth[x, y];

                if (tile.ContainsWall(Direction.Top))
                {
                    walls.Add(new HeroWall(x * Cell, y * Cell, (x + 1) * Cell, y * Cell, walls.Count * WallDrawStep));
                }

                if (tile.ContainsWall(Direction.Left))
                {
                    walls.Add(new HeroWall(x * Cell, y * Cell, x * Cell, (y + 1) * Cell, walls.Count * WallDrawStep));
                }

                if (x == Size - 1 && tile.ContainsWall(Direction.Right))
                {
                    walls.Add(new HeroWall((x + 1) * Cell, y * Cell, (x + 1) * Cell, (y + 1) * Cell, walls.Count * WallDrawStep));
                }

                if (y == Size - 1 && tile.ContainsWall(Direction.Bottom))
                {
                    walls.Add(new HeroWall(x * Cell, (y + 1) * Cell, (x + 1) * Cell, (y + 1) * Cell, walls.Count * WallDrawStep));
                }
            }
        }

        return walls;
    }

    private static List<Position> Walk(Labyrinth labyrinth, int seed, out bool isSolved)
    {
        var visited = new bool[Size, Size];
        Position exit = new(ExitX, ExitY);
        Position start = new(0, 0);

        Stack<Position> stack = new();
        stack.Push(start);
        visited[start.X, start.Y] = true;

        List<Position> walk = [start];
        Random random = new(seed);

        isSolved = false;

        while (stack.Count > 0)
        {
            var current = stack.Peek();

            if (current == exit)
            {
                isSolved = true;
                break;
            }

            var next = FindStep(labyrinth, visited, current, random);

            if (next is null)
            {
                stack.Pop();

                if (stack.Count > 0)
                {
                    walk.Add(stack.Peek());
                }

                continue;
            }

            visited[next.Value.X, next.Value.Y] = true;
            stack.Push(next.Value);
            walk.Add(next.Value);
        }

        return walk;
    }

    private static Position? FindStep(Labyrinth labyrinth, bool[,] visited, Position current, Random random)
    {
        Span<Direction> directions = [Direction.Top, Direction.Right, Direction.Bottom, Direction.Left];
        random.Shuffle(directions);

        foreach (var direction in directions)
        {
            if (labyrinth[current.X, current.Y].ContainsWall(direction))
            {
                continue;
            }

            var candidate = current + direction;

            if (candidate.X < 0 || candidate.Y < 0 || candidate.X >= Size || candidate.Y >= Size)
            {
                continue;
            }

            if (visited[candidate.X, candidate.Y] == false)
            {
                return candidate;
            }
        }

        return null;
    }

    private static string BuildTrailPath(List<Position> walk)
    {
        StringBuilder builder = new();

        for (var index = 0; index < walk.Count; index++)
        {
            var point = walk[index];

            builder.Append(index == 0 ? 'M' : 'L')
                .Append(point.X * Cell + Cell / 2)
                .Append(' ')
                .Append(point.Y * Cell + Cell / 2)
                .Append(' ');
        }

        return builder.ToString().TrimEnd();
    }
}
