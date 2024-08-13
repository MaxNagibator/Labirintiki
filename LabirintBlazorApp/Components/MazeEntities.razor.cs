using LabirintBlazorApp.Common.Drawing;
using LabirintBlazorApp.Components.Base;

namespace LabirintBlazorApp.Components;

public partial class MazeEntities : MazeComponent
{
    private int _entitySize;
    private int _offset;

    protected override string CanvasId => "mazeEntitiesCanvas";

    protected override void OnParametersSetInner()
    {
        int entityBoxSize = BoxSize - WallWidth;
        _entitySize = entityBoxSize / 2;
        _offset = _entitySize - entityBoxSize;
    }

    protected override void DrawInner(int x, int y, DrawSequence sequence)
    {
        Item? item = Maze[x, y].ItemType;

        if (item == null)
        {
            return;
        }

        Position draw = Vision.GetDraw((x, y)) * BoxSize + WallWidth;
        (int left, int top) = AlignmentHelper.CalculatePosition(Alignment.BottomCenter, draw, _offset);

        sequence.DrawImage($"/images/{item.Name}.png", left, top, _entitySize, _entitySize);
    }
}
