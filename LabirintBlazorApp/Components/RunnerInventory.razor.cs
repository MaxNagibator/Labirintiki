using System.Collections.ObjectModel;
using LabirintBlazorApp.Common.Control.Schemes;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class RunnerInventory : RenderComponent, IAsyncDisposable
{
    private ObservableCollection<ShowingStack>? _items;

    [Parameter]
    public required Inventory Inventory { get; set; }

    [Parameter]
    public required KeyInterceptor Interceptor { get; set; }

    [Inject]
    public required IControlSchemeService SchemeService { get; set; }

    private ShowingStack WaitItem { get; set; }

    private IControlScheme ControlScheme => SchemeService.CurrentScheme;

    public async ValueTask DisposeAsync()
    {
        Interceptor.ChangedWaitItem -= OnChangedWaitItem;
        SchemeService.ControlSchemeChanged -= OnSchemeChanged;
        await Interceptor.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected override Task OnRenderAsyncInner()
    {
        return Task.CompletedTask;
    }

    protected override Task OnFirstRenderAsyncInner()
    {
        _items = new ObservableCollection<ShowingStack>(Inventory.Stacks.Select(x => new ShowingStack(x)));

        Interceptor.ChangedWaitItem += OnChangedWaitItem;
        SchemeService.ControlSchemeChanged += OnSchemeChanged;

        Inventory.ItemAdded += async (_, item) =>
        {
            _items.FirstOrDefault(x => x.Stack.Item.Name == item.Name).Add();
            await ForceRenderAsync();
        };

        Inventory.ItemCantAdded += async (_, item) =>
        {
            _items.FirstOrDefault(x => x.Stack.Item.Name == item.Name).CantAdd();
            await ForceRenderAsync();
        };

        Inventory.ItemUsed += async (_, item) =>
        {
            _items.FirstOrDefault(x => x.Stack.Item.Name == item.Name).Use();
            await ForceRenderAsync();
        };

        return Task.CompletedTask;
    }

    private async void OnSchemeChanged(object? sender, IControlScheme scheme)
    {
        await ForceRenderAsync();
    }

    private async void OnChangedWaitItem(object? sender, Item? args)
    {
        if (args == null)
        {
            WaitItem.IsWaiting = false;
        }
        else
        {
            ShowingStack? showingStack = _items?.FirstOrDefault(x => x.Stack.Item.Name == args.Name);

            if (showingStack != null)
            {
                showingStack.Wait();
                WaitItem = showingStack;
            }
        }

        await ForceRenderAsync();
    }

    private void OnClicked(Key activateKey)
    {
        Interceptor.OnKeyDown(activateKey);
    }

    private async Task Added(ShowingStack showingStack)
    {
        showingStack.Clear();
        await ForceRenderAsync();
    }

    private class ShowingStack(ItemStack stack)
    {
        public ItemStack Stack { get; } = stack;
        public bool IsAdded { get; set; }
        public bool IsUsed { get; set; }
        public bool IsCantAdded { get; set; }
        public bool IsWaiting { get; set; }

        public string GetAnimations()
        {
            string result = "";

            if (IsAdded)
            {
                result += " added-animate";
            }

            if (IsUsed)
            {
                result += " used-animate";
            }

            if (IsCantAdded)
            {
                result += " max-count-animate";
            }

            if (IsWaiting)
            {
                result += " waiting-animate";
            }

            return result;
        }

        public void Add()
        {
            IsAdded = true;
        }

        public void Wait()
        {
            IsWaiting = true;
        }

        public void Use()
        {
            IsUsed = true;
        }

        public void CantAdd()
        {
            IsCantAdded = true;
        }

        public void Clear()
        {
            IsAdded = false;
            IsUsed = false;
            IsCantAdded = false;
        }
    }
}
