using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public class ChildEventAggregator : EventAggregator, ISubscriber
    {
        public async Task HandleAsync(IMessage message)
        {
            await this.PublishAsync<IMessage>(message);
        }

        public bool IsAlive => true;
    }
}
