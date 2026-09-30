using Labirint.Core;
using Labirint.Core.TileFeatures.Base;
using Labirint.Core.TileFeatures.Common;
using Labirint.Web.Components;

namespace Labirint.Web.Tests.Components;

[TestFixture]
public class MazeEntitiesTests
{
    /// <summary>
    /// Тестирует, что MazeEntities.CollectDrawingSettings отбирает картинки клетки так же, как прежняя цепочка Where/DistinctBy/OrderBy.
    /// Проверяет, что особенности без DrawingSettings пропускаются, равные настройки схлопываются в первую, а порядок идёт по Order и при равном Order остаётся исходным.
    /// </summary>
    [Test]
    public void CollectsDistinctSettingsInStableOrderTest()
    {
        DrawingSettings yarn = new("yarn.webp", Alignment.Stretch, 1);
        DrawingSettings hammer = new("hammer.webp", Alignment.Center, 0.5, 1);
        DrawingSettings bomb = new("bomb.webp", Alignment.Center, 0.5, 1);
        DrawingSettings mark = new("mark.webp", Alignment.Stretch, 1);

        TileFeature[] features =
        [
            new FakeFeature(hammer),
            new FakeFeature(null),
            new FakeFeature(yarn),
            new FakeFeature(bomb),
            new FakeFeature(yarn with { }),
            new FakeFeature(mark),
        ];

        List<DrawingSettings> settings = [new("stale.webp", Alignment.Stretch, 1)];

        MazeEntities.CollectDrawingSettings(features, settings);

        Assert.That(settings, Is.EqualTo(new[] { yarn, mark, hammer, bomb }));
    }

    /// <summary>
    /// Тестирует, что MazeEntities.CollectDrawingSettings очищает переиспользуемый список для клетки без особенностей.
    /// Проверяет, что при Features == null в списке не остаётся настроек от предыдущей клетки.
    /// </summary>
    [Test]
    public void ClearsSettingsForEmptyTileTest()
    {
        List<DrawingSettings> settings = [new("stale.webp", Alignment.Stretch, 1)];

        MazeEntities.CollectDrawingSettings(null, settings);

        Assert.That(settings, Is.Empty);
    }

    private sealed class FakeFeature(DrawingSettings? drawingSettings) : TileFeature
    {
        public override bool RemoveAfterSuccessPickUp => false;
        public override string PickUpSound => string.Empty;
        public override DrawingSettings? DrawingSettings => drawingSettings;

        public override bool TryPickUp(Labyrinth labyrinth)
        {
            return false;
        }
    }
}
