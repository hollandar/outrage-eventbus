# Outrage EventBus

A general purpose subscribe/publish event bus library that:
* Uses a thread channel to orchestrate message execution
* Supports dependency injection via Microsoft.Extensions.DependencyInjection
* Employs weak references, which are especially useful for UI orchestration; no deregistering subscriptions
* Supports parent/child streams for component specific orchestration

## Getting started
1. Subclass a root event bus from Event Aggregator.  You can have multiple levels of event aggregator subscribed to different events through as many subclasses of event aggregator as you need.
```
public class RootEventBus: EventAggregator, IRootEventBus {
  public RootEventBus(IServiceProvider serviceProvider) : base(serviceProvider) {}
}
```

2. Inject the root event bus into the dependency injection service collection using the IRootEventBus marker interface
```
serviceCollection.AddScoped<IRootEventBus, EventAggregator>();
```

3. Create a message class, or reuse the standard messages classes from Outrage.EventBus.Messages.  Messages should inherit the marker interface IMessage.
```
public class SomeMessage : IMessage {
}
```

4. Subscribe to a message using one of the Subscribe methods.  The following uses an inline async lambda to process messages of the type Outrage.EventBus.Messages.LogMessage.

```
var rootEventBus = serviceProvider.GetService<IRootEventBus>();
var subscriber = rootEventBus.Subscribe<LogMessage>((busContext, logMessage) => {
  Console.WriteLine($"Received log message containing {logMessage.Message}");
  return Task.CompletedTask;
});
```
Note: You should hold a reference to the subscriber until you no longer wish to receive the messages.  The subscription is destroyed when the reference you hold goes out of scope.  There is no need to deregister a subscriber because all subscribers are held by the event bus as a weak reference.

4. Post a message onto the bus.
```
var rootEventBus = serviceProvider.GetService<IRootEventBus>();
rootEventBus.PublishAsync(new LogMessage("A message has been sent."));
```
## Subscribe Methods
```
TSubscriber Subscribe<TSubscriber>(bool subscribed = true) where TSubscriber : ISubscriber;
```
Attach a subscriber to the event bus, constructing the subscriber instance using dependency injection;
returns: The subscriber instance.

```
FilterSubscriber<TMessage> Subscribe<TMessage>(Func<EventContext, TMessage, Task> messageDelegate, bool subscribed = true) where TMessage : IMessage;
```
Subscribe using a method delegate or lambda function, filtering for certain types of message;
returns: the subscriber instance.

```
ISubscriber Subscribe(Func<EventContext, IMessage, Task> messageDelegate, bool subscribed = true);
```
Subscribe using a method delegate or lambda function, to messages of all types.  You should filter for types of messages by testing message is MessageClass or similar;
returns: the subscriber instance.

```
ISubscriber Subscribe<TMessage>(Func<Task> messageDelegate, bool subscribed = true) where TMessage: IMessage;
```
Subscribe using a method delegate or lambda function.  You do not receive the message instance, simply a notification that a message of th type TMessage was received.
returns: the subscriber instance.

```
ISubscriber Subscribe(ISubscriber subscriber);
```
Subscribe by passing an instance of ISubscriber.  You can implement your own instance or utilize FilterSubscriber or InjectSubscriber.
returns: the subscriber instance.
FilterSubscriber: A subscriber that takes a method delegate or lambda function and passes messages of a certain type on to it.
InjectSubscriber: A subscriber that, on receipt of a message, delegates to a subscriber created using dependency injection.

## Bus Heirarchies
A ChildEventAggregator is a special instance of the bus that receives messages from its parent bus, and republishes all messages within the child.  Messages published on the child bus are not propagated back to the parent.

You can either create a general purpose child bus, or derive from ChildEventAggregator to create a specific bus and resolve it using dependency injection.
The CreateChildBus method provides a general bus that is a child to the one from which it was created.
```
IEventAggregator CreateChildBus();
```  

A great use-case from a child bus is to drive the user interface of a component section of an application, in concert with the application root bus.  For example, you could publish log messages into the Root bus (they will also be received by the ui child bus); and toast message events into the ui child bus exclusively for presentation in the user interface.  Because the event buses utilize a threading channel, you may need to marshal child subscribers to the ui thread at runtime.

## Ideas
You can use subscribers and child buses to create orchestrations that:
* perform message translation, posting new messages back into the bus;
* return result success or failure back through the event bus;
* centralize logging into a bus; presenting logging information in a number of receivers; integrating logging specific infrastructure;
* orchestrate complex interactions between decoupled front and back end services;
* subscribers that serialise messages and pass them across network boundaries;
* encapsulate business logic into subscribers;
* create processing chains using follow-on messages published back to the bus
* asynchronously initiate long running workloads; reporting responses once complete via response messages;

## Further examples
More complete examples are to be developed.  Check out the test cases for further examples.
