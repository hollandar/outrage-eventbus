using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public abstract class EventMultiplexer<TMessage>: ISubscriber where TMessage: IMessage
    {
        private readonly IEventAggregator eventAggregator;

        public EventMultiplexer (IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public async Task HandleAsync(EventContext context, IMessage message)
        {
            if (message is TMessage)
            {
                var multiplexMessages = this.Multiplex(context, (TMessage)message);
                foreach (var multiplexMessage in multiplexMessages)
                    await this.eventAggregator.PublishAsync(multiplexMessage);
            }
        }

        protected abstract IEnumerable<IMessage> Multiplex(EventContext context, TMessage message);

        public bool IsAlive => true;
    }
}
