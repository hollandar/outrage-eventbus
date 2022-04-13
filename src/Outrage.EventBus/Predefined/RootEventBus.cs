using System;

namespace Outrage.EventBus.Predefined
{
    public class RootEventBus : EventAggregator, IRootEventBus
    {
        /// <summary>
        /// A root event bus from which others can flow.
        /// Events from from the root, but remain in the children in they are raised there.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public RootEventBus(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
    }
}
