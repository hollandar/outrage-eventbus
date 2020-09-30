using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public class EventAggregator : IEventAggregator
    {
        private static class EventAggregatorInstance {
            public static EventAggregator instance = new EventAggregator();
            public static HashSet<ISubscriber> persistentSubscriptions = new HashSet<ISubscriber>();
        }
        
        public static IEventAggregator Bus { get { return EventAggregatorInstance.instance; } }
        
        public IReadOnlyCollection<ISubscriber> PersistentSubscriptions {  get { return EventAggregatorInstance.persistentSubscriptions.ToList().AsReadOnly(); } }

        public static void MakePersistent(ISubscriber subscriber)
        {
            EventAggregatorInstance.persistentSubscriptions.Add(subscriber);
        }

        public static void MakeTransient(ISubscriber subscriber)
        {
            EventAggregatorInstance.persistentSubscriptions.Remove(subscriber);
        }

        private List<WeakReference<ISubscriber>> subscribers = new List<WeakReference<ISubscriber>>();

        public async Task PublishAsync<TMessage>(TMessage message) where TMessage: IMessage
        {
            var index = 0;
            while (index < subscribers.Count)
            {
                var subscriberReference = subscribers[index];
                if (subscriberReference.TryGetTarget(out ISubscriber subscriber))
                {
                    await subscriber.HandleAsync(message);
                    index++;
                }
                else
                {
                    subscribers.RemoveAt(index);
                }
            }

            GC.Collect();
        }

        public void Subscribe(ISubscriber subscriber)
        {
            this.subscribers.Add(new WeakReference<ISubscriber>(subscriber));
        }
    }
}
