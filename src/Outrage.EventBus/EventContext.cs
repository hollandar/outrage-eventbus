using System;
using System.Collections.Generic;
using System.Text;

namespace Outrage.EventBus
{
    public class EventContext
    {
        public EventContext(IEventAggregator bus, IServiceProvider serviceProvider)
        {
            this.Bus = bus;
            this.ServiceProvider = serviceProvider;
        }

        public IEventAggregator Bus { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
    }
}
