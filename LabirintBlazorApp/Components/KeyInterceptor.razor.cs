using LabirintBlazorApp.Common;
using LabirintBlazorApp.Common.Schemes;
using LabirintBlazorApp.Dto;
using LabirintBlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LabirintBlazorApp.Components;

public partial class KeyInterceptor : IAsyncDisposable
{
    private bool _isPause = false;
    
    private Dictionary<string, Direction> _moveDirections = new();
    private DotNetObjectReference<KeyInterceptor>? _reference;
    private HashSet<string> _attackKeys = [];

    [Inject]
    public required IControlSchemeService SchemeService { get; set; }

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public EventCallback<MoveEventArgs> OnMoveKeyDown { get; set; }

    [Parameter]
    public EventCallback<AttackEventArgs> OnAttackKeyDown { get; set; }

    private IControlScheme ControlScheme => SchemeService.CurrentScheme;

    public async ValueTask DisposeAsync()
    {
        await JSRuntime.InvokeVoidAsync("finalizeKeyInterceptor");
        _reference?.Dispose();
        
        SchemeService.ControlSchemeChanged -= OnSchemeChanged;
        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        _reference = DotNetObjectReference.Create(this);

        Initialize();
        SchemeService.ControlSchemeChanged += OnSchemeChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("initializeKeyInterceptor", _reference);
        }
    }

    private void OnSchemeChanged(object? sender, IControlScheme scheme)
    {
        Initialize();
        StateHasChanged();
    }

    private void Initialize()
    {
        _moveDirections = new Dictionary<string, Direction>
        {
            { ControlScheme.MoveLeft, Direction.Left },
            { ControlScheme.MoveUp, Direction.Top },
            { ControlScheme.MoveRight, Direction.Right },
            { ControlScheme.MoveDown, Direction.Bottom },
        };

        _attackKeys =
        [
            ControlScheme.Molot,
            ControlScheme.Bomba
        ];
    }

    [JSInvokable]
    public async Task OnKeyDown(string code)
    {
        if (_isPause)
        {
            return;
        }

        if (_attackKeys.Contains(code))
        {
            AttackEventArgs attackEventArgs = new()
            {
                KeyCode = Key.Create(code),
                Type = ControlScheme.GetAttackType(code)
            };

            await OnAttackKeyDown.InvokeAsync(attackEventArgs);
        }

        if (_moveDirections.TryGetValue(code, out Direction direction))
        {
            MoveEventArgs moveEventArgs = new()
            {
                Direction = direction,
                KeyCode = Key.Create(code)
            };

            await OnMoveKeyDown.InvokeAsync(moveEventArgs);
        }
    }
}
