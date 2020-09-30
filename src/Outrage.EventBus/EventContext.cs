using System;
using System.Collections.Generic;
using System.Text;

namespace Outrage.EventBus
{
    public class EventContext
    {
        public IEventAggregator Bus { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
    }
}
