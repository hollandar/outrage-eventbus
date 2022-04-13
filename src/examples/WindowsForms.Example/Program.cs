using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Outrage.EventBus;

namespace WindowsForms.Example
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var host = Host.CreateDefaultBuilder();
            host.ConfigureServices((context, services) => {
                services.AddTransient<MessageForm>();
                services.AddEventBus(options => {
                    options.AddDefaultRootBus();
                    options.AddDefaultClientBus();
                });
            });

            var builtHost = host.Build();

            Application.Run(builtHost.Services.GetRequiredService<MessageForm>());
        }
    }
}