using Microsoft.Extensions.DependencyInjection;
using Outrage.EventBus;
using Outrage.EventBus.Options;
using System.Reflection.Metadata.Ecma335;

var serviceCollection = new ServiceCollection();
serviceCollection.AddEventBus(options => options.AddDefaultRootBus());

var serviceProvider = serviceCollection.BuildServiceProvider();

var rootBus = serviceProvider.GetRequiredService<IRootEventBus>();
Stack<ISubscriber> subscribers = new Stack<ISubscriber>();

long count = 0;
while (true)
{
    var next = Random.Shared.Next(0, 3);

    switch (next)
    {
        case 0 when subscribers.Count < 1000:
            var subsciber = rootBus.Subscribe<TestEvent>((v, e) => {
                Interlocked.Increment(ref count);
                if (count % 100_000_000 == 0) 
                    Console.WriteLine($"sid: {count} c: {subscribers.Count} t: {ThreadPool.ThreadCount}");
                return Task.CompletedTask;
            });
            subscribers.Push(subsciber);
            break;
        case 1 when subscribers.Count <= 1000 && subscribers.Count > 900:
            var subscriber = subscribers.Pop();
            rootBus.Unsubscribe(subscriber);
            break;
        case 2:
            Task.Run(async () =>
            {
                await rootBus.PublishAsync(new TestEvent(Random.Shared.Next()));
            });
            break;
    }
}

public record struct TestEvent(long id): IMessage
{
    public long Id { get; init; } = id;
}