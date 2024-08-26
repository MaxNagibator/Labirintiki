namespace Labirint.Core.Items.Common;

public record ControlSettings(Key ActivateKey, Key? AlternativeActivateKey = null, bool MoveRequired = false);
