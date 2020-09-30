using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Outrage.EventBus.Messages;
using Outrage.EventBus.Test.EventBus;
using System;
using System.Threading.Tasks;

namespace Outrage.EventBus.Test
{
    [TestClass]
    public class BusTests
    {
        private ServiceProvider serviceProvider;

        [TestInitialize]
        public void Initialize()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IRootEventBus, TestEventBus>();
            serviceCollection.AddScoped<InternalSubscriber>();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Test for a single filtered subscriber to the bus, as a filtered lambda or delegate
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task FilteredFuncSubscriber()
        {
            var passed = false;
            var eventBus = serviceProvider.GetService<IRootEventBus>();
            var subscriber = eventBus.Subscribe<LogMessage>((context, message) =>
            {
                if (message.Message == "test")
                {
                    passed = true;
                }

                return Task.CompletedTask;
            });

            await eventBus.PublishAsync(new LogMessage("test"));
            await TestHelpers.DelayUntil(() => passed);
            Assert.IsTrue(passed, "Message subscriber was not called.");
        }

        /// <summary>
        /// An injectable custom subscriber, injected from DI
        /// </summary>
        class InternalSubscriber : ISubscriber
        {
            public InternalSubscriber()
            {
                LastMessage = null;
            }

            public static IMessage LastMessage { get; private set; }
            public Task HandleAsync(EventContext context, IMessage message)
            {
                LastMessage = message;
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// A message with a default constructor
        /// </summary>
        class TypedMessage : IMessage
        {

        }

        /// <summary>
        /// Test for a subscriber injected from DI
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DISubscriber()
        {
            var eventBus = serviceProvider.GetService<IRootEventBus>();
            var subscriber = eventBus.Subscribe<InternalSubscriber>();

            await eventBus.PublishAsync(new LogMessage("test"));
            await TestHelpers.DelayUntil(() => InternalSubscriber.LastMessage is LogMessage);
            Assert.IsTrue(InternalSubscriber.LastMessage is LogMessage, "Message subscriber was not called.");
        }

        /// <summary>
        /// Subscribe to the bus, but dont filter, delegate receives all messages
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UnfilteredFuncSubscriber()
        {
            var passed = false;
            var eventBus = serviceProvider.GetService<IRootEventBus>();
            var subscriber = eventBus.Subscribe((context, message) =>
            {
                var logMessage = message as LogMessage;
                if (logMessage?.Message == "test")
                {
                    passed = true;
                }

                return Task.CompletedTask;
            });

            await eventBus.PublishAsync(new LogMessage("test"));
            await TestHelpers.DelayUntil(() => passed);
            Assert.IsTrue(passed, "Message subscriber was not called.");
        }

        /// <summary>
        /// Test for a directly created subscriber
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InstancedSubscriber()
        {
            var eventBus = serviceProvider.GetService<IRootEventBus>();
            var subscriber = new InternalSubscriber();
            eventBus.Subscribe(subscriber);

            await eventBus.PublishAsync(new LogMessage("test"));
            await TestHelpers.DelayUntil(() => InternalSubscriber.LastMessage is LogMessage);
            Assert.IsTrue(InternalSubscriber.LastMessage is LogMessage, "Message subscriber was not called.");
        }

        /// <summary>
        /// Test for a published message that supports new()
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InstancedSubscriberDefaultConstructor()
        {
            var eventBus = serviceProvider.GetService<IRootEventBus>();
            var subscriber = new InternalSubscriber();
            eventBus.Subscribe(subscriber);

            await eventBus.PublishAsync<TypedMessage>();
            await TestHelpers.DelayUntil(() => InternalSubscriber.LastMessage is TypedMessage);
            Assert.IsTrue(InternalSubscriber.LastMessage is TypedMessage, "Message subscriber was not called.");
        }

        /// <summary>
        /// Test for a published message that supports new()
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InstancedSubscriberNotificationOnly()
        {
            var passed = false;
            var eventBus = serviceProvider.GetService<IRootEventBus>();
            var subscriber = eventBus.Subscribe<TypedMessage>(() => { passed = true; return Task.CompletedTask; });

            await eventBus.PublishAsync<TypedMessage>();
            await TestHelpers.DelayUntil(() => passed);
            Assert.IsTrue(passed, "Message subscriber was not called.");
        }

        /// <summary>
        /// Test for a directly created subscriber, but with subscriber loss
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InstancedSubscriberReferenceLoss()
        {
            var eventBus = serviceProvider.GetService<IRootEventBus>();

            Action subscribe = () =>
            {
                var subscriber = new InternalSubscriber();
                eventBus.Subscribe(subscriber);
            };

            subscribe();

            Func<Task> test = async () =>
            {
                GC.Collect();

                //subscriber out of scope and collected, should no longer be called

                await eventBus.PublishAsync(new LogMessage("test"));
                await TestHelpers.DelayUntil(() => InternalSubscriber.LastMessage is LogMessage);
                Assert.IsFalse(InternalSubscriber.LastMessage is LogMessage, "Message subscriber was called.");
            };

            await test();
        }

    }
}
