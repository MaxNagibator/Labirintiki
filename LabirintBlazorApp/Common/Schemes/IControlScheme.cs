namespace LabirintBlazorApp.Common.Schemes;

public interface IControlScheme
{
    string Name { get; }
    Key MoveUp { get; }
    Key MoveDown { get; }
    Key MoveLeft { get; }
    Key MoveRight { get; }
    Key Molot { get; }
    Key Bomba { get; }

    AttackType GetAttackType(string code)
    {
        return code == Molot
            ? AttackType.Molot
            : code == Bomba
                ? AttackType.Bomba
                : AttackType.None;
    }
}
