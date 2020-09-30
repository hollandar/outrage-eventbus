using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Outrage.EventBus.Messages;
using Outrage.EventBus.Test.EventBus;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Outrage.EventBus.Test
{
    [TestClass]
    public class ChildBusTests
    {
        private ServiceProvider serviceProvider;

        [TestInitialize]
        public void Initialize()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IRootEventBus, TestEventBus>();
            serviceCollection.AddScoped<InternalSubscriber>();
            serviceCollection.AddScoped<InternalEventBus>();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        class InternalEventBus:ChildEventAggregator
        {
            public InternalEventBus(IServiceProvider serviceProvider, IRootEventBus rootBus): base(serviceProvider)
            {
                rootBus.Subscribe(new InjectSubscriber<InternalSubscriber>());
            }
        }

        /// <summary>
        /// Test for activation of a child bus against the root bus and injected subscription within the child bus
        /// allowing you to bring collections of subscribers into scope and taken them out of scope by managing the lifetime of the child bus
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ChildBusSubscriberByInjection()
        {
            var eventBus = serviceProvider.GetService<IRootEventBus>();

            // Subscription is an injected subscriber on the now in scope child bus
            var childBus = serviceProvider.GetService<InternalEventBus>();
            
            await eventBus.PublishAsync(new LogMessage("test"));
            await TestHelpers.DelayUntil(() => InternalSubscriber.LastMessage is LogMessage);
            Assert.IsTrue(InternalSubscriber.LastMessage is LogMessage, "Message subscriber was not called.");
        }

        /// <summary>
        /// Test for activation of a child bus against the root bus
        /// allowing you to bring collections of subscribers into scope and taken them out of scope by managing the lifetime of the child bus
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ChildBusSubscriberByCreation()
        {
            var eventBus = serviceProvider.GetService<IRootEventBus>();

            // Subscription is an injected subscriber on the now in scope child bus
            var childBus = eventBus.CreateChildBus();
            var subscriber = childBus.Subscribe(new InternalSubscriber());

            await eventBus.PublishAsync(new LogMessage("test"));
            await TestHelpers.DelayUntil(() => InternalSubscriber.LastMessage is LogMessage);
            Assert.IsTrue(InternalSubscriber.LastMessage is LogMessage, "Message subscriber was not called.");
        }

        /// <summary>
        /// Test for activation of a child bus and event raised against the child to test for back propagation to the parent bus
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ChildBusNoBackprop()
        {
            var eventBus = serviceProvider.GetService<IRootEventBus>();
            var subscriber = eventBus.Subscribe(new InternalSubscriber());

            var childBus = eventBus.CreateChildBus();
            await childBus.PublishAsync(new LogMessage("test"));
            await TestHelpers.DelayUntil(() => InternalSubscriber.LastMessage is LogMessage);
            Assert.IsFalse(InternalSubscriber.LastMessage is LogMessage, "Message subscriber was not called.");
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

        
    }
}
