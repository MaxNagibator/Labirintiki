using BlazorAnimation;

namespace LabirintBlazorApp.Services;

public class AnimationService
{
    private readonly AnimationEffect[] _animateEffects =
    [
        Effect.ShakeX,
        Effect.ShakeX,
        Effect.Tada,
        Effect.Wobble,
        Effect.Jello,
        Effect.RubberBand
    ];

    public AnimationEffect AnimationEffect { get; set; } = Effect.FlipInY;

    public void StartRandomAnimationEffect()
    {
        AnimationEffect = _animateEffects
            .Where(effect => effect != AnimationEffect)
            .ToArray()
            [Random.Shared.Next(_animateEffects.Length - 2)];
    }
}
