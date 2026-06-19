using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Outrage.EventBus;
using Outrage.EventBus.Messages;
using Outrage.EventBus.Options;
using System.Reflection.Metadata.Ecma335;

var serviceCollection = new ServiceCollection();
serviceCollection.AddEventBus(options => { 
    options.AddDefaultRootBus();
    options.AddExceptionPublisher();
});

var serviceProvider = serviceCollection.BuildServiceProvider();

var rootBus = serviceProvider.GetRequiredService<IRootEventBus>();
Stack<ISubscriber> subscribers = new Stack<ISubscriber>();

#pragma warning disable CS4014

long count = 0;
int threadCount = 1000;
long exceptionCount = 0;
int mag = 1000;
TestEvent testEvent = new TestEvent(0);
int subscribedCount = 0;

rootBus.Subscribe<EventBusExceptionMessage>((v, e) =>
{
    Interlocked.Increment(ref exceptionCount);
    if (exceptionCount % (threadCount * mag) == 0)
        Console.WriteLine($"{exceptionCount} exceptions handled.");
    return Task.CompletedTask;
});

while (true)
{
    var next = Random.Shared.Next(0, 3);

    switch (next)
    {
        case 0 when subscribedCount < threadCount:
            var subsciber = rootBus.Subscribe<TestEvent>((v, e) =>
            {
                Interlocked.Increment(ref count);
                if (count % (threadCount * mag )== 0)
                    Console.WriteLine($"sid: {count} c: {subscribers.Count} t: {ThreadPool.ThreadCount}");
                if (Random.Shared.Next(0, 10) == 0) 
                    throw new InvalidOperationException();
                return Task.CompletedTask;
            });
            subscribers.Push(subsciber);
            subscribedCount++;
            break;
        case 1 when subscribedCount > (int)(threadCount * 0.9):
            var subscriber = subscribers.Pop();
            if (Random.Shared.Next(0, 10) == 0)
                rootBus.Unsubscribe(subscriber);
            subscribedCount--;
            break;
        case 2:
            Task.Run(async () =>
            {
                await rootBus.PublishAsync(testEvent);
            });
            break;
    }
}

 
public record TestEvent(long id) : IMessage
{
    public long Id { get; init; } = id;
}