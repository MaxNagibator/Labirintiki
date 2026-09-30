namespace Labirint.Web.Tests.Drawing;

[TestFixture]
public class DrawSequenceTests
{
    private const string Floor = "images/floor.png";
    private const string Wall = "images/wall.png";

    /// <summary>
    /// Тестирует, что DrawSequence.DrawSprite схлопывает идущие подряд спрайты с одним источником в одну команду DrawSprites.
    /// Проверяет, что для двух спрайтов из одного файла итоговая команда одна, с общим Source и плоским массивом из 16 чисел по 8 на спрайт.
    /// </summary>
    [Test]
    public void SpritesWithSameSourceCollapseTest()
    {
        DrawSequence sequence = new();

        sequence.DrawSprite(Floor, 0d, 0d, 16, 16, 0, 0, 64, 64);
        sequence.DrawSprite(Floor, 16d, 0d, 16, 16, 64, 0, 64, 64);

        var commands = sequence.Build();

        Assert.That(commands, Has.Count.EqualTo(1));

        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Type, Is.EqualTo(DrawSequence.Command.DrawSprites));
            Assert.That(commands[0].Source, Is.EqualTo(Floor));
            Assert.That(commands[0].Sprites, Is.EqualTo(new double[] { 0, 0, 16, 16, 0, 0, 64, 64, 16, 0, 16, 16, 64, 0, 64, 64 }));
        });
    }

    /// <summary>
    /// Тестирует, что DrawSequence.DrawSprite при смене источника спрайта завершает накопленную команду и начинает новую.
    /// Проверяет, что для спрайтов из разных файлов получаются две отдельные команды DrawSprites со своим Source и восемью числами каждая.
    /// </summary>
    [Test]
    public void SourceChangeFlushesSpritesTest()
    {
        DrawSequence sequence = new();

        sequence.DrawSprite(Floor, 0d, 0d, 16, 16, 0, 0, 64, 64);
        sequence.DrawSprite(Wall, 0d, 0d, 16, 16, 0, 0, 64, 64);

        var commands = sequence.Build();

        Assert.That(commands, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Source, Is.EqualTo(Floor));
            Assert.That(commands[0].Sprites, Has.Length.EqualTo(8));
            Assert.That(commands[1].Source, Is.EqualTo(Wall));
            Assert.That(commands[1].Sprites, Has.Length.EqualTo(8));
        });
    }

    /// <summary>
    /// Тестирует, что DrawSequence перед любой не-спрайтовой командой сбрасывает накопленную группу спрайтов.
    /// Проверяет, что при чередовании DrawSprite и Stroke порядок команд в Build сохраняет DrawSprites до и после Stroke раздельно.
    /// </summary>
    [Test]
    public void OtherCommandFlushesSpritesBeforeItselfTest()
    {
        DrawSequence sequence = new();

        sequence.DrawSprite(Floor, 0d, 0d, 16, 16, 0, 0, 64, 64);
        sequence.Stroke();
        sequence.DrawSprite(Floor, 0d, 0d, 16, 16, 0, 0, 64, 64);

        var commands = sequence.Build();

        Assert.That(commands.Select(command => command.Type),
            Is.EqualTo(new[] { DrawSequence.Command.DrawSprites, DrawSequence.Command.Stroke, DrawSequence.Command.DrawSprites }));
    }

    /// <summary>
    /// Тестирует, что DrawSequence.DrawSprite переводит номер строки и столбца спрайт-листа в пиксельное смещение источника.
    /// Проверяет, что для строки 2 и столбца 3 при размере спрайта 10x20 в команде оказывается смещение источника 30 и 40 пикселей.
    /// </summary>
    [Test]
    public void RowAndColumnAreTranslatedToSourceOffsetTest()
    {
        DrawSequence sequence = new();

        sequence.DrawSprite(Floor, 2, 3, 10, 20, 100, 200, 64, 64);

        var commands = sequence.Build();

        Assert.That(commands[0].Sprites, Is.EqualTo(new double[] { 30, 40, 10, 20, 100, 200, 64, 64 }));
    }

    /// <summary>
    /// Тестирует, что DrawSequence.DrawSprite для квадратного спрайта использует один общий размер как для источника, так и для места на канвасе.
    /// Проверяет, что перегрузка с единственным размером даёт одинаковую пару "ширина, высота" в обеих частях массива Sprites.
    /// </summary>
    [Test]
    public void SquareSpriteKeepsSizeOnBothSidesTest()
    {
        DrawSequence sequence = new();

        sequence.DrawSprite(Floor, 1, 1, 128, 256, 64);

        var commands = sequence.Build();

        Assert.That(commands[0].Sprites, Is.EqualTo(new double[] { 64, 64, 64, 64, 128, 256, 64, 64 }));
    }

    /// <summary>
    /// Тестирует, что DrawSequence.Build для последовательности без единой команды не создаёт лишних записей.
    /// Проверяет, что Build на пустой последовательности возвращает пустую коллекцию команд.
    /// </summary>
    [Test]
    public void EmptySequenceProducesNoCommandsTest()
    {
        DrawSequence sequence = new();

        Assert.That(sequence.Build(), Is.Empty);
    }
}
