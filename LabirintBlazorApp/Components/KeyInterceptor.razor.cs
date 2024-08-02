using LabirintBlazorApp.Common;
using LabirintBlazorApp.Common.Schemes;
using LabirintBlazorApp.Dto;
using LabirintBlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LabirintBlazorApp.Components;

public partial class KeyInterceptor : IDisposable
{
    private Dictionary<string, (int DeltaX, int DeltaY)> _moveDirections;
    private ElementReference _keyInterceptorRef;
    private HashSet<string> _attackKeys;

    [Inject]
    public required IControlSchemeService SchemeService { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback<MoveEventArgs> OnMoveKeyDown { get; set; }

    [Parameter]
    public EventCallback<AttackEventArgs> OnAttackKeyDown { get; set; }

    private IControlScheme ControlScheme => SchemeService.CurrentScheme;

    public void Dispose()
    {
        SchemeService.ControlSchemeChanged -= OnSchemeChanged;
        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        Initialize();
        SchemeService.ControlSchemeChanged += OnSchemeChanged;
    }

    private void OnSchemeChanged(object? sender, IControlScheme scheme)
    {
        Initialize();
        StateHasChanged();
    }

    private void Initialize()
    {
        _moveDirections = new Dictionary<string, (int, int)>
        {
            { ControlScheme.MoveUp, (0, -1) },
            { ControlScheme.MoveDown, (0, 1) },
            { ControlScheme.MoveLeft, (-1, 0) },
            { ControlScheme.MoveRight, (1, 0) }
        };

        _attackKeys =
        [
            ControlScheme.Molot,
            ControlScheme.Bomba
        ];
    }

    public async Task FocusAsync()
    {
        if (_keyInterceptorRef is { Id: not null, Context: not null })
        {
            await _keyInterceptorRef.FocusAsync();
        }
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (_attackKeys.Contains(e.Code))
        {
            AttackEventArgs attackEventArgs = new()
            {
                KeyCode = Key.Create(e.Code),
                Type = ControlScheme.GetAttackType(e.Code)
            };

            await OnAttackKeyDown.InvokeAsync(attackEventArgs);
        }

        if (_moveDirections.TryGetValue(e.Code, out (int DeltaX, int DeltaY) direction))
        {
            MoveEventArgs moveEventArgs = new()
            {
                Direction = direction,
                KeyCode = Key.Create(e.Code)
            };

            await OnMoveKeyDown.InvokeAsync(moveEventArgs);
        }
    }
}
