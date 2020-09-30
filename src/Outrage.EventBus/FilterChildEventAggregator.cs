using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public class FilterChildEventAggregator<TMessage> : EventAggregator, ISubscriber where TMessage: IMessage
    {
        public virtual async Task HandleAsync(IMessage message)
        {
            if (message is TMessage)
            {
                await this.PublishAsync<TMessage>((TMessage)message);
            }
        }

        public bool IsAlive => true;
    }
}
