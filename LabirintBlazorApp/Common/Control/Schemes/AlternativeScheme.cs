using Labirint.Core.Items.Common;

namespace LabirintBlazorApp.Common.Control.Schemes;

public class AlternativeScheme : IControlScheme
{
    public string Name => "Альтернативная схема (WASD)";

    public Key MoveUp => Key.KeyW;
    public Key MoveDown => Key.KeyS;
    public Key MoveLeft => Key.KeyA;
    public Key MoveRight => Key.KeyD;

    public Key GetActivateKey(ControlSettings settings)
    {
        return settings.AlternativeActivateKey ?? settings.ActivateKey;
    }
}
