using Labirint.Web.Components.Dialogs;
using Labirint.Web.Services.Dialogs;

namespace Labirint.Web.Tests.Services;

[TestFixture]
public class DialogServiceTests
{
    /// <summary>
    /// Тестирует, что DialogService.CancelOwnedBy закрывает только диалоги указанного владельца.
    /// Проверяет, что оба диалога владельца завершают ShowAsync отменой и уходят из стека, а диалог другого владельца остаётся открытым и его ShowAsync не завершён.
    /// </summary>
    [Test]
    public async Task CancelOwnedByClosesOnlyOwnDialogsTest()
    {
        DialogService service = new();
        object owner = new();
        object otherOwner = new();

        var first = service.ShowAsync<ShareDialog>(owner, "Первый");
        var other = service.ShowAsync<ShareDialog>(otherOwner, "Чужой");
        var second = service.ShowAsync<ShareDialog>(owner, "Второй");

        service.CancelOwnedBy(owner);

        var results = await Task.WhenAll(first, second);

        Assert.Multiple(() =>
        {
            Assert.That(results.Select(result => result.Canceled), Is.All.True);
            Assert.That(service.Instances.Select(instance => instance.Title), Is.EqualTo(new[] { "Чужой" }));
            Assert.That(other.IsCompleted, Is.False);
        });
    }
}
