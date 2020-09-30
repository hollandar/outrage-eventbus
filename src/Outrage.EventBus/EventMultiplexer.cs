using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public abstract class EventMultiplexer<TMessage>: ISubscriber where TMessage: IMessage
    {
        public async Task HandleAsync(IMessage message)
        {
            if (message is TMessage)
            {
                var multiplexMessages = this.Multiplex((TMessage)message);
                foreach (var multiplexMessage in multiplexMessages)
                    await EventAggregator.Bus.PublishAsync(multiplexMessage);
            }
        }

        protected abstract IEnumerable<IMessage> Multiplex(TMessage message);

        public bool IsAlive => true;
    }
}
