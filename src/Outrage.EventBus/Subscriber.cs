using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public class Subscriber : ISubscriber, IDisposable
    {
        public Subscriber(Func<IMessage, Task> onMessage)
        {
            this.onMessage = onMessage;
        }

        Func<IMessage, Task> onMessage;

        public async Task HandleAsync(IMessage message)
        {
            await onMessage(message);
        }

        void IDisposable.Dispose() { }
    }
}
