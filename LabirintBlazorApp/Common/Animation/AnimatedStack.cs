namespace LabirintBlazorApp.Common.Animation;

public class AnimatedStack(ItemStack stack)
{
    private readonly List<State> _states = [];

    public enum State
    {
        None,
        Added,
        Used,
        CantAdd,
        Waiting
    }

    public ItemStack Stack { get; } = stack;

    public string GetAnimations()
    {
        return string.Join(" ", _states.Select(state => state.ToAnimation()));
    }

    public void AddState(State state)
    {
        _states.Add(state);
    }

    // TODO Проблемы при завершении нескольких анимаций одновременно
    public void RemoveState()
    {
        if (_states.Count > 0)
        {
            _states.RemoveAt(0);
        }
    }
}
