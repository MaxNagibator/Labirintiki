namespace Labirint.Web.Services;

public class AnimationService
{
    private const string EntranceClass = "maze-anim--flip";

    private readonly string[] _effectClasses =
    [
        "maze-anim--shake",
        "maze-anim--tada",
        "maze-anim--wobble",
        "maze-anim--jello",
        "maze-anim--rubber",
    ];

    public string EffectClass { get; private set; } = EntranceClass;

    public void StartRandomAnimationEffect()
    {
        string[] candidates = _effectClasses.Where(effectClass => effectClass != EffectClass).ToArray();

        EffectClass = candidates[Random.Shared.Next(candidates.Length)];
    }
}
