using System;

namespace Outrage.EventBus.Test.EventBus
{
    public class TestEventBus:EventAggregator, IRootEventBus
    {
        public TestEventBus(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }
}
