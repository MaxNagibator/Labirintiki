using Labirint.Web.Common.Animation;
using Labirint.Web.Common.Control.Schemes;
using Labirint.Web.Components.Dialogs;
using Labirint.Web.Services.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Components;

public partial class RunnerInventory : RenderComponent, IDisposable
{
    private InventoryAnimator _animator = null!;

    private Item? _pendingFlight;
    private Item? _pendingCast;

    [Parameter]
    [EditorRequired]
    public required Inventory Inventory { get; set; }

    [Parameter]
    [EditorRequired]
    public required KeyInterceptor Interceptor { get; set; }

    [Inject]
    public required ControlSchemeService SchemeService { get; set; }

    [Inject]
    public required PickupFlightService PickupFlight { get; set; }

    [Inject]
    public required DialogService DialogService { get; set; }

    private IControlScheme ControlScheme => SchemeService.CurrentScheme;

    public void Dispose()
    {
        DialogService.CancelOwnedBy(this);

        Interceptor.ChangedWaitItem -= OnChangedWaitItem;
        Interceptor.DigitKeyDown -= OnDigitKeyDown;
        SchemeService.ControlSchemeChanged -= OnSchemeChanged;

        _animator.Changed -= OnAnimatorChanged;
        _animator.PickupFlightRequested -= OnPickupFlightRequested;
        _animator.CastFlightRequested -= OnCastFlightRequested;
        _animator.Dispose();

        GC.SuppressFinalize(this);
    }

    public bool TryStartCast(Item item)
    {
        return _animator.TryStartCast(item);
    }

    public void CancelCast(Item item)
    {
        _animator.CancelCast(item);
    }

    protected override void OnInitialized()
    {
        _animator = new InventoryAnimator(Inventory);
        _animator.Changed += OnAnimatorChanged;
        _animator.PickupFlightRequested += OnPickupFlightRequested;
        _animator.CastFlightRequested += OnCastFlightRequested;

        Interceptor.ChangedWaitItem += OnChangedWaitItem;
        Interceptor.DigitKeyDown += OnDigitKeyDown;
        SchemeService.ControlSchemeChanged += OnSchemeChanged;
    }

    protected override Task OnFirstRenderAsyncInner()
    {
        return Task.CompletedTask;
    }

    protected override Task OnRenderAsyncInner()
    {
        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsyncInner(bool firstRender)
    {
        if (_pendingFlight != null)
        {
            var item = _pendingFlight;
            _pendingFlight = null;

            await PickupFlight.FlyAsync(item);
        }

        if (_pendingCast != null)
        {
            var item = _pendingCast;
            _pendingCast = null;

            await PickupFlight.CastAsync(item);
        }
    }

    private void OnAnimatorChanged()
    {
        RunSafe(ForceRenderAsync);
    }

    private void OnPickupFlightRequested(Item item)
    {
        _pendingFlight = item;
    }

    private void OnCastFlightRequested(Item item)
    {
        _pendingCast = item;
    }

    private void OnSchemeChanged(object? sender, IControlScheme scheme)
    {
        RunSafe(ForceRenderAsync);
    }

    private void OnChangedWaitItem(object? sender, Item? item)
    {
        _animator.SetWaitItem(item);
    }

    private void OnDigitKeyDown(object? sender, DigitEventArgs args)
    {
        var control = Inventory.Stacks
            .Where(stack => stack.Count > 0)
            .ElementAtOrDefault(args.Digit - 1)
            ?.Item.ControlSettings;

        if (control != null)
        {
            Interceptor.OnKeyDown(control.ActivateKey);
        }
    }

    private string? GetKeySymbol(Item item)
    {
        return item.ControlSettings == null
            ? null
            : ControlScheme.GetActivateKey(item.ControlSettings).DisplaySymbol;
    }

    private async Task ShowLoreAsync(Item item)
    {
        var animatedStack = _animator.Find(item);

        if (animatedStack == null)
        {
            return;
        }

        DialogParameters parameters = new()
        {
            [nameof(ItemLoreDialog.Item)] = item,
            [nameof(ItemLoreDialog.Count)] = animatedStack.Stack.Count,
            [nameof(ItemLoreDialog.IsInfinite)] = animatedStack.Stack.IsInfinite,
        };

        var result = await DialogService.ShowAsync<ItemLoreDialog>(this, item.DisplayName, parameters, new DialogOptions
        {
            Width = DialogWidth.Medium,
        });

        if (result.GetValue<bool>())
        {
            OnClicked(item);
        }
    }

    private void OnClicked(Item item)
    {
        if (item.ControlSettings != null)
        {
            Interceptor.OnKeyDown(ControlScheme.GetActivateKey(item.ControlSettings));
        }
    }
}
