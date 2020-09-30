using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public class ChildEventAggregator : EventAggregator, ISubscriber
    {
        public ChildEventAggregator(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public async Task HandleAsync(EventContext context, IMessage message)
        {
            await this.PublishAsync<IMessage>(message);
        }

        public bool IsAlive => true;
    }
}
