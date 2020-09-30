using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Outrage.EventBus
{
    public abstract class EventAggregator : IEventAggregator, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private readonly List<WeakReference<ISubscriber>> subscribers = new List<WeakReference<ISubscriber>>();
        private readonly Channel<IMessage> messageChannel = Channel.CreateUnbounded<IMessage>();
        private readonly CancellationTokenSource channelReadCancellationSource = new CancellationTokenSource();

        private Task channelReaderTask = null;

        protected EventAggregator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public TSubscriber Subscribe<TSubscriber>(bool subscribed = true) where TSubscriber : ISubscriber
        {
            var subscriber = this.serviceProvider.GetRequiredService<TSubscriber>();
            if (subscribed) this.Subscribe(subscriber);
            return subscriber;
        }

        public FilterSubscriber<TMessage> Subscribe<TMessage>(Func<EventContext, TMessage, Task> messageDelegate, bool subscribed = true) where TMessage : IMessage
        {
            var filter = new FilterSubscriber<TMessage>(messageDelegate);
            if (subscribed) this.Subscribe(filter);
            return filter;
        }

        public FilterSubscriber<TMessage> Subscribe<TMessage>(Func<Task> messageDelegate, bool subscribed = true) where TMessage : IMessage
        {
            return this.Subscribe<TMessage>((ctx, msg) => { messageDelegate(); return Task.CompletedTask; }, subscribed);
        }

        public Subscriber Subscribe(Func<EventContext, IMessage, Task> messageDelegate, bool subscribed = true)
        {
            var subscriber = new Subscriber(messageDelegate);
            if (subscribed) this.Subscribe(subscriber);
            return subscriber;
        }

        public void Subscribe(ISubscriber subscriber)
        {
            this.subscribers.Insert(0, new WeakReference<ISubscriber>(subscriber));
        }

        public async Task PublishAsync<TMessage>() where TMessage : IMessage, new()
        {
            var message = new TMessage();
            await this.PublishAsync(message);
        }

        public Task PublishAsync<TMessage>(TMessage message) where TMessage : IMessage
        {
            if (channelReaderTask == null) channelReaderTask = Task.Run(ProcessPublishQueue);
            this.messageChannel.Writer.TryWrite(message);
            return Task.CompletedTask;
        }

        public async Task ProcessPublishQueue()
        {
            CancellationToken cancellationToken = channelReadCancellationSource.Token;
            List<Exception> exceptionsThrown = new List<Exception>();
            while (await this.messageChannel.Reader.WaitToReadAsync(cancellationToken))
            {
                var message = await this.messageChannel.Reader.ReadAsync();

                var context = new EventContext { Bus = this, ServiceProvider = this.serviceProvider };
                var index = 0;
                while (index < subscribers.Count)
                {
                    var subscriberReference = subscribers[index];
                    if (subscriberReference.TryGetTarget(out ISubscriber subscriber))
                    {
                        try
                        {
                            await subscriber.HandleAsync(context, message);
                        }
                        catch (Exception e)
                        {
                            if (e is ConvertableBusException)
                            {
                                var convertableException = e as ConvertableBusException;
                                var convertedMessage = convertableException.Convert(message);
                                await this.PublishAsync(convertedMessage);
                            }
                            else
                            {
                                // Hold exceptions thrown
                                exceptionsThrown.Add(e);
                            }
                        }
                        index++;
                    }
                    else
                    {
                        subscribers.RemoveAt(index);
                    }

                    // Now throw any process exceptions as an aggregate
                    if (exceptionsThrown.Any())
                        throw new AggregateException(exceptionsThrown);
                }
            }
        }

        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool managed)
        {
            if (managed)
            {
                this.channelReadCancellationSource.Cancel();
            }
        }
    }
}
