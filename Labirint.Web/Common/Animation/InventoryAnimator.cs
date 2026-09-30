namespace Labirint.Web.Common.Animation;

public sealed class InventoryAnimator : IDisposable
{
    private readonly Inventory _inventory;

    private Dictionary<Item, AnimatedStack> _stacks;
    private AnimatedStack? _waitStack;
    private Item? _castingItem;

    public InventoryAnimator(Inventory inventory)
    {
        _inventory = inventory;
        _stacks = CreateStacks();

        _inventory.ItemAdded += OnItemAdded;
        _inventory.ItemCantAdded += OnItemCantAdded;
        _inventory.ItemUsed += OnItemUsed;
        _inventory.ItemCantUsed += OnItemCantUsed;
        _inventory.InventoryCleared += OnInventoryCleared;
    }

    public event Action? Changed;
    public event Action<Item>? PickupFlightRequested;
    public event Action<Item>? CastFlightRequested;

    public IReadOnlyCollection<AnimatedStack> Stacks => _stacks.Values;

    public void Dispose()
    {
        ReleaseStacks();

        _inventory.ItemAdded -= OnItemAdded;
        _inventory.ItemCantAdded -= OnItemCantAdded;
        _inventory.ItemUsed -= OnItemUsed;
        _inventory.ItemCantUsed -= OnItemCantUsed;
        _inventory.InventoryCleared -= OnInventoryCleared;
    }

    public AnimatedStack? Find(Item item)
    {
        return _stacks.GetValueOrDefault(item);
    }

    public bool TryStartCast(Item item)
    {
        if (_castingItem != null || _stacks.TryGetValue(item, out var stack) == false)
        {
            return false;
        }

        _castingItem = item;
        CastFlightRequested?.Invoke(item);

        stack.AddState(AnimatedStack.State.Used);
        stack.ReserveCount();

        Changed?.Invoke();
        return true;
    }

    public void CancelCast(Item item)
    {
        if (_castingItem != item)
        {
            return;
        }

        _castingItem = null;
        Find(item)?.SyncCount();
    }

    public void SetWaitItem(Item? item)
    {
        if (item == null)
        {
            _waitStack?.CancelState();
            _waitStack = null;
            return;
        }

        if (_stacks.TryGetValue(item, out var stack) == false)
        {
            return;
        }

        stack.AddState(AnimatedStack.State.Waiting);
        _waitStack = stack;
    }

    private Dictionary<Item, AnimatedStack> CreateStacks()
    {
        var stacks = _inventory.Stacks.ToDictionary(stack => stack.Item, stack => new AnimatedStack(stack));

        foreach (var stack in stacks.Values)
        {
            stack.StateChanged += OnStateChanged;
        }

        return stacks;
    }

    private void ReleaseStacks()
    {
        foreach (var stack in _stacks.Values)
        {
            stack.StateChanged -= OnStateChanged;
        }
    }

    private void OnStateChanged(AnimatedStack.State state)
    {
        Changed?.Invoke();
    }

    private void OnItemAdded(object? sender, Item item)
    {
        AddState(item, AnimatedStack.State.Added);

        PickupFlightRequested?.Invoke(item);
        Changed?.Invoke();
    }

    private void OnItemCantAdded(object? sender, Item item)
    {
        AddState(item, AnimatedStack.State.CantAdd);
    }

    private void OnItemUsed(object? sender, Item item)
    {
        if (_castingItem == item)
        {
            _castingItem = null;
            Find(item)?.SyncCount();

            Changed?.Invoke();
            return;
        }

        AddState(item, AnimatedStack.State.Used);

        CastFlightRequested?.Invoke(item);
        Changed?.Invoke();
    }

    private void OnItemCantUsed(object? sender, Item item)
    {
        CancelCast(item);
        AddState(item, AnimatedStack.State.CantUse);
    }

    private void OnInventoryCleared(object? sender, EventArgs args)
    {
        ReleaseStacks();
        _stacks = CreateStacks();
        _waitStack = null;
        _castingItem = null;

        Changed?.Invoke();
    }

    private void AddState(Item item, AnimatedStack.State state)
    {
        Find(item)?.AddState(state);
    }
}
