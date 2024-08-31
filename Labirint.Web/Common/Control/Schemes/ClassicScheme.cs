using Labirint.Core.Items.Common;

namespace Labirint.Web.Common.Control.Schemes;

public class ClassicScheme : IControlScheme
{
    public string Name => "Классическая схема (←↕→)";

    public Key MoveUp => Key.ArrowUp;
    public Key MoveDown => Key.ArrowDown;
    public Key MoveLeft => Key.ArrowLeft;
    public Key MoveRight => Key.ArrowRight;

    public Key GetActivateKey(ControlSettings settings)
    {
        return settings.ActivateKey;
    }
}
