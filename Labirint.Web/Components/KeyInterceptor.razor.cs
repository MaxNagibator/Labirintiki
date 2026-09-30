using Labirint.Web.Common.Control.Schemes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Labirint.Web.Components;

public partial class KeyInterceptor : IAsyncDisposable
{
    private readonly string _interceptorId = Guid.NewGuid().ToString("N");
    private readonly KeyBindingMap _bindings = new();

    private bool _isPause;

    private DotNetObjectReference<KeyInterceptor>? _reference;
    private Item? _waitItem;

    public event EventHandler<AttackEventArgs>? AttackKeyDown;
    public event EventHandler<Item?>? ChangedWaitItem;
    public event EventHandler<DigitEventArgs>? DigitKeyDown;
    public event EventHandler<MoveEventArgs>? MoveKeyDown;

    [Parameter]
    public required Inventory? Inventory { get; set; }

    [Inject]
    public required ControlSchemeService SchemeService { get; set; }

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Inject]
    public required ILogger<KeyInterceptor> Logger { get; set; }

    public bool IsPaused => _isPause;

    private IControlScheme ControlScheme => SchemeService.CurrentScheme;

    public async ValueTask DisposeAsync()
    {
        SchemeService.ControlSchemeChanged -= OnSchemeChanged;

        try
        {
            await JSRuntime.InvokeVoidAsync("finalizeKeyInterceptor", _interceptorId);
        }
        catch (JSException exception)
        {
            Logger.LogError(exception, "Не удалось снять перехватчик клавиш");
        }

        _reference?.Dispose();
        _reference = null;

        GC.SuppressFinalize(this);
    }

    public void InitializeItems()
    {
        _bindings.RebuildItems(ControlScheme, Inventory);
    }

    public void ResetWaitItem()
    {
        if (_waitItem != null)
        {
            ChangeWaitItem(null);
        }
    }

    [JSInvokable]
    public void OnKeyDown(string code)
    {
        if (_isPause)
        {
            return;
        }

        if (_waitItem == null)
        {
            PerformItemUse(code);
        }

        var direction = _bindings.FindDirection(code);

        if (direction != null)
        {
            PerformMove(new()
            {
                Direction = direction.Value,
            });
        }

        var digit = KeyBindingMap.FindDigit(code);

        if (digit != null)
        {
            DigitKeyDown?.Invoke(this, new()
            {
                Digit = digit.Value,
            });
        }
    }

    public void OnKeyDown(Direction direction)
    {
        PerformMove(new()
        {
            Direction = direction,
        });
    }

    protected override void OnParametersSet()
    {
        InitializeItems();
    }

    protected override void OnInitialized()
    {
        _reference = DotNetObjectReference.Create(this);

        _bindings.Rebuild(ControlScheme, Inventory);
        SchemeService.ControlSchemeChanged += OnSchemeChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("initializeKeyInterceptor", _reference, _interceptorId);
        }
    }

    private void OnSchemeChanged(object? sender, IControlScheme scheme)
    {
        _bindings.Rebuild(ControlScheme, Inventory);
        StateHasChanged();
    }

    private void PerformItemUse(string code)
    {
        var item = _bindings.FindItem(code);

        if (item == null || (Inventory?.CanUse(item) ?? false) == false)
        {
            return;
        }

        if (item.ControlSettings!.MoveRequired)
        {
            ChangeWaitItem(item);
            return;
        }

        AttackKeyDown?.Invoke(this, new()
        {
            Item = item,
        });
    }

    private void PerformMove(MoveEventArgs move)
    {
        if (_isPause)
        {
            return;
        }

        if (_waitItem != null)
        {
            var item = _waitItem;
            ChangeWaitItem(null);

            AttackKeyDown?.Invoke(this, new()
            {
                Item = item,
                Direction = move.Direction,
            });

            return;
        }

        MoveKeyDown?.Invoke(this, move);
    }

    private void ChangeWaitItem(Item? item)
    {
        _waitItem = item;
        ChangedWaitItem?.Invoke(this, item);
    }
}
