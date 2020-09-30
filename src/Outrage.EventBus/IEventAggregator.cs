using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public interface IEventAggregator
    {
        TSubscriber Subscribe<TSubscriber>(bool subscribed = true) where TSubscriber : ISubscriber;
        FilterSubscriber<TMessage> Subscribe<TMessage>(Func<EventContext, TMessage, Task> messageDelegate, bool subscribed = true) where TMessage : IMessage;
        ISubscriber Subscribe(Func<EventContext, IMessage, Task> messageDelegate, bool subscribed = true);
        ISubscriber Subscribe<TMessage>(Func<Task> messageDelegate, bool subscribed = true) where TMessage: IMessage;
        IEventAggregator CreateChildBus();
        Task PublishAsync<TMessage>(TMessage message) where TMessage : IMessage;
        Task PublishAsync<TMessage>() where TMessage : IMessage, new();
        ISubscriber Subscribe(ISubscriber subscriber);
    }
}
