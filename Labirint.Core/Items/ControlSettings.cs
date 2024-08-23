namespace Labirint.Core.Items;

public record ControlSettings(Key ActivateKey, Key? AlternativeActivateKey = null, bool MoveRequired = false);
