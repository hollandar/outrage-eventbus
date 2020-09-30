using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public interface IEventAggregator
    {
        Task PublishAsync<TMessage>(TMessage message) where TMessage : IMessage;
        void Subscribe(ISubscriber subscriber);
    }
}
