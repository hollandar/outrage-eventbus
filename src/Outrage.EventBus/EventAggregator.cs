// Define timer to include messages per second logging into the processor for testing purposes
// #define TIMER

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Outrage.EventBus.Messages;
using Outrage.EventBus.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Outrage.EventBus
{
    public abstract class EventAggregator : IEventAggregator, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<EventAggregator>? logger;
        private readonly List<WeakReference<ISubscriber>> subscribers;
        private Channel<IMessage> messageChannel;
        private readonly CancellationTokenSource channelReadCancellationSource = new CancellationTokenSource();
        private bool logEnabled = false;
        private bool logExceptionEnabled = false;
        private volatile bool subscriptionsChanged = false;
        private volatile bool channelReaderRunning = false;
        ISubscriber? exceptionSubscriber;
        ISubscriber? logSubscriber;

        private Task? channelReaderTask = null;
        private readonly ReaderWriterLockSlim subscriberLock = new ReaderWriterLockSlim();
        private readonly SemaphoreSlim channelCreationLock = new SemaphoreSlim(1);

        const double garbagePressure = 0.1f;

        protected EventAggregator(IServiceProvider serviceProvider)
        {
            this.subscribers = new List<WeakReference<ISubscriber>>();
            this.messageChannel = Channel.CreateUnbounded<IMessage>();

            this.serviceProvider = serviceProvider;
            this.logger = this.serviceProvider.GetService<ILogger<EventAggregator>>();
            var options = this.serviceProvider.GetService<EventBusOptions>();
            if (options != null)
            {
                if (options.DefaultExceptionSubscriber) this.AddDefaultExceptionSubscriber(options.DefaultExceptionMessage);
                if (options.DefaultLoggingSubscriber) this.AddDefaultLogSubscriber();
                if (options.ExceptionPublisher) this.AddExceptionPublisher();
                if (options.LoggingPublisher) this.AddLoggingPublisher();
            }
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

        public ISubscriber Subscribe(Func<EventContext, IMessage, Task> messageDelegate, bool subscribed = true)
        {
            var subscriber = new Subscriber(messageDelegate);
            if (subscribed) this.Subscribe(subscriber);
            return subscriber;
        }

        public ISubscriber Subscribe<TMessage>(Func<Task> messageDelegate, bool subscribed = true) where TMessage : IMessage
        {
            return this.Subscribe<TMessage>(async (context, message) => { await messageDelegate(); });
        }

        public ISubscriber Subscribe(ISubscriber subscriber)
        {
            try
            {
                subscriberLock.EnterWriteLock();
                this.subscribers.Add(new WeakReference<ISubscriber>(subscriber, false));
                subscriptionsChanged = true;
                return subscriber;
            }
            finally
            {
                subscriberLock.ExitWriteLock();
            }
        }

        public void Unsubscribe(ISubscriber subscriberTarget)
        {
            try
            {
                subscriberLock.EnterUpgradeableReadLock();
                var references = this.subscribers.Where(reference =>
                {
                    if (reference.TryGetTarget(out var subscriber))
                    {
                        return subscriber == subscriberTarget;
                    }

                    return false;
                }).ToArray();

                if (references.Length > 0)
                {
                    try
                    {
                        subscriberLock.EnterWriteLock();
                        foreach (var subscriberReference in references)
                        {
                            this.subscribers.Remove(subscriberReference);
                            subscriptionsChanged = true;
                        }
                    }
                    finally
                    {
                        subscriberLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                subscriberLock.ExitUpgradeableReadLock();
            }
        }

        public IEventAggregator CreateChildBus()
        {
            var child = new ChildEventAggregator(this.serviceProvider);
            this.Subscribe(child);
            return child;
        }

        public async Task PublishAsync<TMessage>() where TMessage : IMessage, new()
        {
            var message = new TMessage();
            await this.PublishAsync(message);
        }

        public Task PublishAsync<TMessage>(TMessage message) where TMessage : IMessage
        {
            if (logEnabled)
                this.messageChannel.Writer.TryWrite(new EventBusLogMessage() { Level = LogLevel.Debug, Message = $"Message published with type {message.GetType().FullName}." });

            if (this.messageChannel.Writer.TryWrite(message))
            {
                if (channelReaderTask == null || !channelReaderRunning)
                {
                    try
                    {
                        channelCreationLock.Wait();
                        if (channelReaderTask == null || !channelReaderRunning)
                            channelReaderTask = Task.Run(ProcessPublishQueue);
                    }
                    finally { channelCreationLock.Release(); }
                }
            }
            else
            {
                // message channel writer has been marked as completed, recreate a new message channel
                messageChannel = Channel.CreateUnbounded<IMessage>();
                this.logger?.LogWarning("EventBus channel was recreated after the channel writer was closed");
            }
            return Task.CompletedTask;
        }

        public async Task ProcessPublishQueue()
        {
            try
            {
                channelReaderRunning = true;
                CancellationToken cancellationToken = channelReadCancellationSource.Token;
                var invalidSubscribers = new Queue<WeakReference<ISubscriber>>();
                var context = new EventContext(this, this.serviceProvider, cancellationToken);
                IReadOnlyCollection<WeakReference<ISubscriber>>? subscribersSnapshot = null;
                int targetCount = 0;

                while (await this.messageChannel.Reader.WaitToReadAsync(cancellationToken))
                {
#if TIMER
                var timer = Stopwatch.StartNew();
                long msgCount = 0;
#endif
                    while (this.messageChannel.Reader.TryRead(out IMessage? message))
                    {
                        // Subscriptions have changed, build a new snapshot of subscribers
                        if (subscribersSnapshot is null || subscriptionsChanged)
                        {
                            try
                            {
                                subscriberLock.EnterReadLock();
                                subscribersSnapshot = this.subscribers.GetRange(0, this.subscribers.Count).AsReadOnly();
                                targetCount = (int)(this.subscribers.Count * garbagePressure);
                                subscriptionsChanged = false;
                            }
                            finally { subscriberLock.ExitReadLock(); }
                        }

                        // Post the message to each subscriber in a separate task and track any invalid subscribers that are found during processing to be cleaned up after processing completes to avoid locking the subscriber list during processing
                        foreach (var subscriberReference in subscribersSnapshot)
                        {
                            if (cancellationToken.IsCancellationRequested) { break; }

                            if (subscriberReference.TryGetTarget(out ISubscriber subscriber))
                            {
                                _ = Task.Run(async () =>
                                {
                                    await subscriber.HandleAsync(context, message);
                                }).ContinueWith(async (t) =>
                                {
                                    if (t.IsFaulted)
                                    {
                                        var e = t.Exception.InnerException;
                                        if (e is ConvertableBusException)
                                        {
                                            var convertableException = e as ConvertableBusException;
                                            var convertedMessage = convertableException!.Convert(message);
                                            await this.PublishAsync(convertedMessage);
                                        }
                                        else
                                        {
                                            // Log any thrown exceptions
                                            if (logExceptionEnabled)
                                            {
                                                    this.logger?.LogError(e, "Exception thrown processing event chain.");
                                                    await this.PublishAsync<EventBusExceptionMessage>(
                                                        new EventBusExceptionMessage(new AggregateException(t.Exception.InnerExceptions))
                                                    );
                                                }
                                            }
                                        }

                                });
                            }
                            else
                            {
                                invalidSubscribers.Enqueue(subscriberReference);
                            }
                        }

                        // Now throw any process exceptions as an aggregate

                        // Clean up any invalid subscribers that were found during processing back to a baseline
                        if (invalidSubscribers.Count > (targetCount * 4))
                        {
                            CleanupInvalidSubscribers(invalidSubscribers, targetCount);
                        }
#if TIMER
                    Interlocked.Increment(ref msgCount);
                    if (msgCount % 10000 == 0) {
                        Console.WriteLine($"Msg / sec = {msgCount / timer.Elapsed.TotalSeconds}");
                    }
#endif
                    }

                    // Clean up all invalid subscribers before waiting for the next message if there are any left
                    CleanupInvalidSubscribers(invalidSubscribers, 0);

                }
            }
            finally
            {
                channelReaderRunning = false;
            }
        }

        private void CleanupInvalidSubscribers(Queue<WeakReference<ISubscriber>> invalidSubscribers, int targetCount)
        {
            try
            {
                subscriberLock.EnterWriteLock();
                this.logger?.LogDebug($"Cleaning up {invalidSubscribers.Count} invalid subscriber references.");
                while (invalidSubscribers.Count > targetCount)
                {
                    if (invalidSubscribers.TryDequeue(out var invalidSubscriber))
                    {
                        this.subscribers.Remove(invalidSubscriber);
                        subscriptionsChanged = true;
                    }
                }
            }
            finally { subscriberLock.ExitWriteLock(); }
        }

        public void AddDefaultExceptionSubscriber(string msg = "Exception thrown processing event chain.")
        {
            if (this.logger == null)
            {
                throw new LoggerNotInjectedException("Adding default exception logging, no logging service has been injected.");
            }
            this.exceptionSubscriber = this.Subscribe<EventBusExceptionMessage>((eventContext, eventMessage) =>
            {
                this.logger.LogError(eventMessage.Exception, msg ?? eventMessage.Exception.Message);
                return Task.CompletedTask;
            });
            this.logExceptionEnabled = true;
        }

        public void AddExceptionPublisher() => this.logExceptionEnabled = true;

        public void AddDefaultLogSubscriber()
        {
            if (this.logger == null)
            {
                throw new LoggerNotInjectedException("Adding default logging, no logging service has been injected.");
            }
            this.logSubscriber = this.Subscribe<EventBusLogMessage>((eventContext, eventMessage) =>
            {
                this.logger.Log(eventMessage.Level, eventMessage.Message);
                return Task.CompletedTask;
            });
            this.logEnabled = true;
        }

        public void AddLoggingPublisher() => this.logEnabled = true;

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
