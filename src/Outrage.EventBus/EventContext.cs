using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Outrage.EventBus
{
    public class EventContext
    {
        public EventContext(IEventAggregator bus, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            this.Bus = bus;
            this.ServiceProvider = serviceProvider;
            this.CancellationToken = cancellationToken;
        }

        public IEventAggregator Bus { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
    }
}
