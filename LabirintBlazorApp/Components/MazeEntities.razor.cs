using LabirintBlazorApp.Common;
using LabirintBlazorApp.Dto;

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
        if (Maze[x, y].HasSand == false)
        {
            return;
        }

        Position draw = Vision.GetDraw((x, y)) * BoxSize + WallWidth;

        (int left, int top) = AlignmentHelper.CalculatePosition(Alignment.BottomCenter, draw, _offset);

        sequence.DrawImage("/images/sand.png", left, top, _entitySize, _entitySize);
    }
}
