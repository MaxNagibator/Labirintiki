namespace LabirintBlazorApp.Common.Control.Schemes;

public class AlternativeScheme : IControlScheme
{
    public string Name => "Альтернативная схема";

    public Key MoveUp => Key.KeyW;
    public Key MoveDown => Key.KeyS;
    public Key MoveLeft => Key.KeyA;
    public Key MoveRight => Key.KeyD;

    public Key Molot => Key.Space;
    public Key Bomba => Key.ControlLeft;
}
