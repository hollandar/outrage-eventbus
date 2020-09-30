using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public class FilterSubscriber<TMessage> : ISubscriber where TMessage : IMessage
    {
        public FilterSubscriber(Func<TMessage, Task> onMessage)
        {
            this.onMessage = onMessage;
        }
        protected Func<TMessage, Task> onMessage;

        public async Task HandleAsync(IMessage message)
        {
            if (message is TMessage)
            {
                await onMessage((TMessage)message);
            }
        }
    }
}