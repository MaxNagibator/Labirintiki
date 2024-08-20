namespace LabirintBlazorApp.Parameters;

public class LabyrinthParameters
{
    public const string LocalStorageKey = nameof(LabyrinthParameters);

    public string Color { get; set; } = "#8b0000";
    public bool IsSoundOn { get; set; } = true;
    public float SoundVolume { get; set; } = 1.0f;
}
