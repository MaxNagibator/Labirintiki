namespace Labirint.Web.Common.Hero;

public sealed record HeroRun(
    int Seed,
    int Side,
    HeroExit Exit,
    IReadOnlyList<HeroWall> Walls,
    string TrailPath,
    int TrailLength,
    int TailLength,
    int TrailDelay,
    int TrailDuration,
    bool IsSolved)
{
    public int Duration => TrailDelay + TrailDuration;

    public string Style =>
        $"--route: path('{TrailPath}'); --trail-length: {TrailLength}; --tail-length: {TailLength}; " +
        $"--trail-delay: {TrailDelay}ms; --trail-duration: {TrailDuration}ms; " +
        $"--found-delay: {Duration}ms";
}
