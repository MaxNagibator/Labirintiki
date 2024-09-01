using Labirint.Core.Items.Common;

namespace Labirint.Web.Common.Control.Schemes;

public interface IControlScheme
{
    string Name { get; }

    Key MoveUp { get; }
    Key MoveDown { get; }
    Key MoveLeft { get; }
    Key MoveRight { get; }

    Key GetActivateKey(ControlSettings settings);
}
