using Labirint.Core.Interfaces;

namespace Labirint.Web.Parameters;

public sealed class MazeSession : IDisposable
{
    private const int BoxSize = 64;
    private const int WallWidth = BoxSize / 10;

    public MazeSession(IRandom seeder)
    {
        Labyrinth = new(seeder);
        Labyrinth.RunnerMoved += OnRunnerMoved;
        Labyrinth.ExitFound += OnExitFound;
    }

    public event EventHandler? Finished;

    public Labyrinth Labyrinth { get; }

    public Runner Runner => Labyrinth.Runner;

    public Vision Vision { get; private set; } = null!;

    public MazeRenderParameters Parameters { get; private set; } = null!;

    public int MoveCount { get; private set; }

    public bool IsExitFound { get; private set; }

    public bool IsContinued { get; private set; }

    public bool IsReady { get; private set; }

    public void Dispose()
    {
        Labyrinth.RunnerMoved -= OnRunnerMoved;
        Labyrinth.ExitFound -= OnExitFound;
    }

    public async Task GenerateAsync(int size, int density, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        IsExitFound = false;
        IsContinued = false;
        MoveCount = 0;

        await Labyrinth.InitAsync(size, size, density, progress: progress, cancellationToken: cancellationToken);

        Vision = new(size, size);
        Vision.SetPosition(Runner.Position);

        Parameters = new(Labyrinth, BoxSize, WallWidth, Vision);
        IsReady = true;
    }

    public void Continue()
    {
        IsContinued = true;
        IsExitFound = false;
    }

    private void OnRunnerMoved(object? sender, Position position)
    {
        MoveCount++;
        Vision.SetPosition(position);
    }

    private void OnExitFound(object? sender, EventArgs args)
    {
        if (IsContinued)
        {
            return;
        }

        IsExitFound = true;
        Finished?.Invoke(this, EventArgs.Empty);
    }
}
