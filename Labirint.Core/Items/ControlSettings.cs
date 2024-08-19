namespace Labirint.Core.Items;

public record ControlSettings(Key ActivateKey, Key AlternativeActivateKey, bool MoveRequired = false);
