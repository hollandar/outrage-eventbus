using System;
using System.Collections.Generic;
using System.Text;

namespace Outrage.EventBus.Predefined
{
    /// <summary>
    /// A child event bus that connects to the registered root event bus.
    /// Events on the child are not propagated to the parent, but from the parent to the child.
    /// This makes the child event bus excellent for client or UI thread messaging.
    /// </summary>
    public class ClientEventBus : ChildEventAggregator, IClientEventBus
    {
        public ClientEventBus(IServiceProvider serviceProvider, IRootEventBus rootEventBus) : base(serviceProvider)
        {
            rootEventBus.Subscribe(this);
        }
    }
}
