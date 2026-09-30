using Bunit;
using Labirint.Web.Components.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Labirint.Web.Tests.Components;

[TestFixture]
public class RenderComponentTests
{
    private BunitContext _context = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new();
        _context.Services.AddLogging();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Тестирует, что RenderComponent при первом рендере вызывает и OnFirstRenderAsyncInner, и OnRenderAsyncInner ровно один раз.
    /// Проверяет, что FirstRenderCount и DrawCount после начального рендера компонента равны единице.
    /// </summary>
    [Test]
    public void FirstRenderDrawsOnceTest()
    {
        var component = _context.Render<ProbeComponent>().Instance;

        Assert.Multiple(() =>
        {
            Assert.That(component.FirstRenderCount, Is.EqualTo(1));
            Assert.That(component.DrawCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Тестирует, что RenderComponent.ForceRenderAsync пропускает через ShouldRender ровно одну перерисовку.
    /// Проверяет, что после вызова счётчик рендеров увеличивается на единицу, а DrawCount доходит до двух.
    /// </summary>
    [Test]
    public async Task ForceRenderDrawsAgainTest()
    {
        var rendered = _context.Render<ProbeComponent>();
        var renderCount = rendered.RenderCount;

        await rendered.InvokeAsync(rendered.Instance.ForceRenderAsync);

        Assert.Multiple(() =>
        {
            Assert.That(rendered.Instance.DrawCount, Is.EqualTo(2));
            Assert.That(rendered.RenderCount, Is.EqualTo(renderCount + 1));
        });
    }

    /// <summary>
    /// Тестирует, что RenderComponent.ShouldRender блокирует StateHasChanged, вызванный не через ForceRenderAsync.
    /// Проверяет, что число рендеров и DrawCount не меняются, если флаг перерисовки не был выставлен заранее.
    /// </summary>
    [Test]
    public async Task StateHasChangedWithoutForceIsSkippedTest()
    {
        var rendered = _context.Render<ProbeComponent>();
        var renderCount = rendered.RenderCount;

        await rendered.InvokeAsync(rendered.Instance.RequestStateHasChanged);

        Assert.Multiple(() =>
        {
            Assert.That(rendered.RenderCount, Is.EqualTo(renderCount));
            Assert.That(rendered.Instance.DrawCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Тестирует, что RenderComponent пропускает по одной перерисовке на каждый последовательный ForceRenderAsync, не съедая наложившиеся вызовы.
    /// Проверяет, что два подряд вызова ForceRenderAsync дают два дополнительных рендера и три вызова OnRenderAsyncInner суммарно.
    /// </summary>
    [Test]
    public async Task SequentialForceRendersDrawBothFramesTest()
    {
        var rendered = _context.Render<ProbeComponent>();
        var renderCount = rendered.RenderCount;

        await rendered.InvokeAsync(rendered.Instance.ForceRenderAsync);
        await rendered.InvokeAsync(rendered.Instance.ForceRenderAsync);

        Assert.Multiple(() =>
        {
            Assert.That(rendered.RenderCount, Is.EqualTo(renderCount + 2));
            Assert.That(rendered.Instance.DrawCount, Is.EqualTo(3));
        });
    }

    private sealed class ProbeComponent : RenderComponent
    {
        public int DrawCount { get; private set; }
        public int FirstRenderCount { get; private set; }

        public void RequestStateHasChanged()
        {
            StateHasChanged();
        }

        protected override Task OnRenderAsyncInner()
        {
            DrawCount++;
            return Task.CompletedTask;
        }

        protected override Task OnFirstRenderAsyncInner()
        {
            FirstRenderCount++;
            return Task.CompletedTask;
        }
    }
}
