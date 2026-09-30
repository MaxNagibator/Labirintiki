using Labirint.Core.TileFeatures.Base;
using Labirint.Core.TileFeatures.Common;
using Labirint.Web.Common.Extensions;

namespace Labirint.Web.Components;

public partial class MazeEntities : MazeComponent
{
    private readonly List<DrawingSettings> _settings = [];

    protected override string CanvasId => "mazeEntitiesCanvas";

    public static void CollectDrawingSettings(IReadOnlyList<TileFeature>? features, List<DrawingSettings> settings)
    {
        settings.Clear();

        if (features == null)
        {
            return;
        }

        foreach (var feature in features)
        {
            var setting = feature.DrawingSettings;

            if (setting == null || settings.Contains(setting))
            {
                continue;
            }

            var index = settings.Count;

            while (index > 0 && settings[index - 1].Order > setting.Order)
            {
                index--;
            }

            settings.Insert(index, setting);
        }
    }

    protected override void DrawInner(int x, int y, DrawSequence sequence)
    {
        CollectDrawingSettings(Maze[x, y].Features, _settings);

        if (_settings.Count == 0)
        {
            return;
        }

        var draw = Vision.GetDraw((x, y));

        foreach (var setting in _settings)
        {
            sequence.DrawImage(setting, BoxSize, WallWidth, draw);
        }
    }
}
