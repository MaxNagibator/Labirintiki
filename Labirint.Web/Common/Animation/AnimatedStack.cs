using System.Collections.Concurrent;

namespace Labirint.Web.Common.Animation;

public class AnimatedStack(ItemStack stack)
{
    private readonly ConcurrentQueue<State> _stateQueue = new();
    private State _executedState = State.Removed;
    private int _countVersion;

    public event Action<State>? StateChanged;

    public enum State
    {
        None = 0,
        Added = 1,
        Used = 2,
        CantAdd = 3,
        Waiting = 4,
        Removed = 5,
        CantUse = 6,
    }

    public ItemStack Stack { get; } = stack;

    public int DisplayCount { get; private set; } = stack.Count;

    public bool IsIdle => _stateQueue.IsEmpty && ExecutedState == State.Removed;

    private State ExecutedState
    {
        get => _executedState;
        set
        {
            if (_executedState == value)
            {
                return;
            }

            _executedState = value;
            StateChanged?.Invoke(_executedState);
        }
    }

    public string GetAnimation()
    {
        if (ExecutedState is State.Removed && _stateQueue.TryDequeue(out var state))
        {
            ExecutedState = state;
            ScheduleRemove(state);
        }

        return ExecutedState.ToAnimation();
    }

    public int GetDuration()
    {
        return ExecutedState.ToDuration();
    }

    public int GetDelay()
    {
        return ExecutedState.ToDelay();
    }

    public void AddState(State state)
    {
        if (_stateQueue.Contains(state) || ExecutedState == state)
        {
            SyncDisplayCount(state);
            return;
        }

        _stateQueue.Enqueue(state);
        SyncDisplayCount(state);

        StateChanged?.Invoke(_executedState);
    }

    public void CancelState()
    {
        _stateQueue.Clear();
        ExecutedState = State.Removed;
    }

    public void SyncCount()
    {
        _countVersion++;
        SetDisplayCount(Stack.Count);
    }

    public void ReserveCount()
    {
        _countVersion++;
        SetDisplayCount(Math.Max(0, Stack.Count - 1));
    }

    private void SyncDisplayCount(State state)
    {
        var delay = state.ToDelay();
        var version = ++_countVersion;

        if (delay <= 0)
        {
            SetDisplayCount(Stack.Count);
            return;
        }

        _ = SyncDisplayCountAfterAsync(delay, version);
    }

    private async Task SyncDisplayCountAfterAsync(int delay, int version)
    {
        await Task.Delay(delay);

        if (version != _countVersion)
        {
            return;
        }

        SetDisplayCount(Stack.Count);
    }

    private void SetDisplayCount(int count)
    {
        if (DisplayCount == count)
        {
            return;
        }

        DisplayCount = count;
        StateChanged?.Invoke(_executedState);
    }

    private void ScheduleRemove(State state)
    {
        if (state.IsRepeating() || state.ToDuration() <= 0)
        {
            return;
        }

        _ = RemoveAfterAsync(state);
    }

    private async Task RemoveAfterAsync(State state)
    {
        await Task.Delay(state.ToDuration() + state.ToDelay());

        if (ExecutedState == state)
        {
            ExecutedState = State.Removed;
        }
    }
}
