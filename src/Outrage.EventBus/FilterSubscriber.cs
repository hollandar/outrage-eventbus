using System;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public class FilterSubscriber<TMessage> : ISubscriber where TMessage : IMessage
    {
        public FilterSubscriber(Func<EventContext, TMessage, Task> onMessage)
        {
            this.onMessage = onMessage;
        }
        protected Func<EventContext, TMessage, Task> onMessage;

        public Task HandleAsync(EventContext context, IMessage message)
        {
            if (message is TMessage)
            {
                return onMessage(context, (TMessage)message);
            }

            return Task.CompletedTask;
        }
    }
}