using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public class InjectSubscriber<TSubscriber> : ISubscriber, IDisposable where TSubscriber: ISubscriber
    {
        public InjectSubscriber()
        {
        }

        public async Task HandleAsync(EventContext context, IMessage message)
        {
            var subscriber = context.ServiceProvider.GetRequiredService<TSubscriber>();
            await subscriber.HandleAsync(context, message);
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
