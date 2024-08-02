namespace LabirintBlazorApp.Common.Schemes;

public class ClassicScheme : IControlScheme
{
    public string Name => "Классическая схема";

    public Key MoveUp => Key.ArrowUp;
    public Key MoveDown => Key.ArrowDown;
    public Key MoveLeft => Key.ArrowLeft;
    public Key MoveRight => Key.ArrowRight;

    public Key Molot => Key.KeyA;
    public Key Bomba => Key.KeyB;
}
