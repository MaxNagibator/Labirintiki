using Labirint.Core.Items.Base;
using Labirint.Items.Flame;
using Labirint.Web;
using Labirint.Web.Services.Dialogs;
using Labirint.Web.Services.Toasts;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

ItemCatalog.AddSource(typeof(Flamethrower).Assembly);

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<LocalStorageService>();

builder.Services.AddSingleton<LabyrinthParametersService>();
builder.Services.AddSingleton<SoundService>();
builder.Services.AddSingleton<AnimationService>();
builder.Services.AddSingleton<ControlSchemeService, ControlSchemeService>();
builder.Services.AddScoped<ClipboardService, ClipboardService>();
builder.Services.AddScoped<PickupFlightService>();
builder.Services.AddScoped<MotionService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<AppUpdateService>();

var host = builder.Build();

await host.Services.GetRequiredService<LabyrinthParametersService>().InitializeAsync();

await host.RunAsync();
