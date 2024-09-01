using Labirint.Core.TileFeatures.Common;
using Labirint.Web.Common.Extensions;

namespace Labirint.Web.Components;

public partial class MazeEntities : MazeComponent
{
    protected override string CanvasId => "mazeEntitiesCanvas";

    protected override void DrawInner(int x, int y, DrawSequence sequence)
    {
        IEnumerable<DrawingSettings>? settings = Maze[x, y]
            .Features
            ?.Where(feature => feature.DrawingSettings != null)
            .DistinctBy(feature => feature.DrawingSettings)
            .OrderBy(feature => feature.DrawingSettings!.Order)
            .Select(feature => feature.DrawingSettings!);

        foreach (DrawingSettings setting in settings ?? [])
        {
            Position draw = Vision.GetDraw((x, y));
            sequence.DrawImage(setting, BoxSize, WallWidth, draw);
        }
    }
}
