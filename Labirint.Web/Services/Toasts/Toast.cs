using Labirint.Web.Common.Ui;

namespace Labirint.Web.Services.Toasts;

public sealed record Toast(Guid Id, string Message, UiSeverity Severity);
