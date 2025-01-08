using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.TransformersJS.Demo;
using SpawnDev.BlazorJS.TransformersJS.Demo.Layout.AppTray;
using SpawnDev.BlazorJS.TransformersJS.Demo.Layout;
using SpawnDev.BlazorJS.WebWorkers;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazorJSRuntime(out var JS);
builder.Services.AddWebWorkerService();

if (JS.IsWindow)
{
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
}

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<AppTrayService>();
builder.Services.AddScoped<MainLayoutService>();
builder.Services.AddScoped<ThemeTrayIconService>();

await builder.Build().BlazorJSRunAsync();
