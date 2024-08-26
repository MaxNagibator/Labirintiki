using LabirintBlazorApp.Common.Animation;
using LabirintBlazorApp.Common.Control.Schemes;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class RunnerInventory : RenderComponent, IAsyncDisposable
{
    private Dictionary<Item, AnimatedStack> _stackCache = new();

    [Parameter]
    [EditorRequired]
    public required Inventory Inventory { get; set; }

    [Parameter]
    [EditorRequired]
    public required KeyInterceptor Interceptor { get; set; }

    [Inject]
    public required IControlSchemeService SchemeService { get; set; }

    private AnimatedStack? WaitItem { get; set; }

    private IControlScheme ControlScheme => SchemeService.CurrentScheme;

    public async ValueTask DisposeAsync()
    {
        UnsubscribeEvents();
        await Interceptor.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected override Task OnFirstRenderAsyncInner()
    {
        InitializeItems();
        SubscribeEvents();

        return Task.CompletedTask;
    }

    protected override Task OnRenderAsyncInner()
    {
        return Task.CompletedTask;
    }

    private async void OnSchemeChanged(object? sender, IControlScheme scheme)
    {
        await ForceRenderAsync();
    }

    private async void OnChangedWaitItem(object? sender, Item? item)
    {
        if (item == null)
        {
            WaitItem?.RemoveState();
        }
        else
        {
            if (_stackCache.TryGetValue(item, out AnimatedStack? showingStack))
            {
                showingStack.AddState(AnimatedStack.State.Waiting);
                WaitItem = showingStack;
            }
        }

        await ForceRenderAsync();
    }

    private async void OnItemAdded(object? sender, Item item)
    {
        await AddStackAnimation(item, AnimatedStack.State.Added);
    }

    private async void OnItemCantAdded(object? sender, Item item)
    {
        await AddStackAnimation(item, AnimatedStack.State.CantAdd);
    }

    private async void OnItemUsed(object? sender, Item item)
    {
        await AddStackAnimation(item, AnimatedStack.State.Used);
    }

    private async void OnInventoryCleared(object? sender, EventArgs e)
    {
        InitializeItems();
        await ForceRenderAsync();
    }

    private void OnClicked(Key activateKey)
    {
        Interceptor.OnKeyDown(activateKey);
    }

    private void InitializeItems()
    {
        _stackCache = Inventory.Stacks.ToDictionary(stack => stack.Item, stack => new AnimatedStack(stack));
    }

    private void SubscribeEvents()
    {
        Interceptor.ChangedWaitItem += OnChangedWaitItem;
        SchemeService.ControlSchemeChanged += OnSchemeChanged;

        Inventory.ItemAdded += OnItemAdded;
        Inventory.ItemCantAdded += OnItemCantAdded;
        Inventory.ItemUsed += OnItemUsed;
        Inventory.InventoryCleared += OnInventoryCleared;
    }

    private void UnsubscribeEvents()
    {
        Interceptor.ChangedWaitItem -= OnChangedWaitItem;
        SchemeService.ControlSchemeChanged -= OnSchemeChanged;

        Inventory.ItemAdded -= OnItemAdded;
        Inventory.ItemCantAdded -= OnItemCantAdded;
        Inventory.ItemUsed -= OnItemUsed;
        Inventory.InventoryCleared -= OnInventoryCleared;
    }

    private async Task AddStackAnimation(Item item, AnimatedStack.State animation)
    {
        if (_stackCache.TryGetValue(item, out AnimatedStack? stack) == false)
        {
            return;
        }

        stack.AddState(animation);
        await ForceRenderAsync();
    }

    private async Task AnimationCompleted(AnimatedStack animatedStack)
    {
        animatedStack.RemoveState();
        await ForceRenderAsync();
    }
}
