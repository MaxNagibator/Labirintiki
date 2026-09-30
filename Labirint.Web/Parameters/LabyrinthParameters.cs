namespace Labirint.Web.Parameters;

public class LabyrinthParameters
{
    public const string LocalStorageKey = nameof(LabyrinthParameters);
    public const string DefaultColor = "#a8442c";
    public const string LegacyDefaultColor = "#8b0000";

    private float _soundVolume = 1.0f;

    public string Color { get; set; } = DefaultColor;
    public bool IsSoundOn { get; set; } = true;

    public float SoundVolume
    {
        get => _soundVolume;
        set => _soundVolume = float.IsNaN(value) ? 1.0f : Math.Clamp(value, 0.0f, 1.0f);
    }

    public LabyrinthParameters Clone()
    {
        return new()
        {
            Color = Color,
            IsSoundOn = IsSoundOn,
            SoundVolume = SoundVolume,
        };
    }
}
