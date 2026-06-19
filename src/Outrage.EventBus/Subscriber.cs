using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public class Subscriber : ISubscriber, IDisposable
    {
        public Subscriber(Func<EventContext, IMessage, Task> onMessage)
        {
            this.onMessage = onMessage;
        }

        readonly Func<EventContext, IMessage, Task> onMessage;

        public Task HandleAsync(EventContext context, IMessage message)
        {
            return onMessage(context, message);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool managed)
        {

        }
    }
}
