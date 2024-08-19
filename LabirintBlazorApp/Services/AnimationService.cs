using BlazorAnimation;

namespace LabirintBlazorApp.Services;

public class AnimationService
{
    public AnimationEffect AnimationEffect { get; set; } = Effect.FlipInY;
    private readonly IReadOnlyList<AnimationEffect> _animateEffects =
    [
        Effect.ShakeX,
        Effect.ShakeX,
        Effect.Tada,
        Effect.Wobble,
        Effect.Jello,
        Effect.RubberBand
    ];
    public void StartRandomAnimationEffect()
    {
        AnimationEffect = _animateEffects
            .Where(x=>x != AnimationEffect)
            .ToList()
            [Random.Shared.Next(_animateEffects.Count - 2)]; 
    }
}
