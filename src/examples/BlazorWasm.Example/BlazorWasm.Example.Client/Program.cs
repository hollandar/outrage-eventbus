using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Outrage.EventBus;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddEventBus(options => options.AddDefaultRootBus().AddDefaultClientBus());

await builder.Build().RunAsync();
