using Microsoft.Extensions.DependencyInjection;
using Outrage.EventBus.Options;
using Outrage.EventBus.Predefined;
using System;

namespace Outrage.EventBus
{
    public static class StartupExtensions
    {
        public static void AddEventBus(this IServiceCollection services, Action<EventBusOptions>? options = null)
        {
            var eventBusOptions = new EventBusOptions();
            if (options != null)
                options(eventBusOptions);
            
            services.AddSingleton(eventBusOptions);

            if (eventBusOptions.RegisterDefaultRoot)
                services.AddSingleton<IRootEventBus, RootEventBus>();

            if (eventBusOptions.RegisterDefaultClient)
                services.AddSingleton<IClientEventBus, ClientEventBus>();
        }
    }
}
