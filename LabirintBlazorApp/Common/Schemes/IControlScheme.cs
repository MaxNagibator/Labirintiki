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
        if (code == Molot)
        {
            return AttackType.Molot;
        }

        if (code == Bomba)
        {
            return AttackType.Bomba;
        }

        return AttackType.None;
    }
}
