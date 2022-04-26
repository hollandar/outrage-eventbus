# Outrage EventBus

A general purpose subscribe/publish event bus library that:
* Uses a thread channel to orchestrate message execution
* Supports dependency injection via Microsoft.Extensions.DependencyInjection
* Employs weak references, which are especially useful for UI orchestration; no deregistering subscriptions or unsubscribing delegates on disposal
* Supports parent/child streams for component specific orchestration
* Handles exceptions and logging during event processing

## Getting started

1. Inject the root event bus into the dependency injection service collection using the IRootEventBus marker interface
```c#
serviceCollection.AddScoped<IRootEventBus, Outrage.EventBus.Predefined.RootEventBus>();
```
You can also add it using the AddDefaultRootBus call when setting options via the startup extension 
```c#
services.AddEventBus((options) => { options.AddRootBus(); });
```
2. Create a message class, or reuse the standard messages classes from Outrage.EventBus.Messages.  Messages should inherit the marker interface IMessage.
```c#
public class SomeMessage : IMessage {
}
```

3. Subscribe to a message using one of the Subscribe methods.  The following uses an inline async lambda to process messages of the type Outrage.EventBus.Messages.LogMessage.

```c#
var rootEventBus = serviceProvider.GetService<IRootEventBus>();
var subscriber = rootEventBus.Subscribe<LogMessage>((busContext, logMessage) => {
  Console.WriteLine($"Received log message containing {logMessage.Message}");
  return Task.CompletedTask;
});
```
Note: You should hold a reference to the subscriber until you no longer wish to receive the messages.  The subscription is destroyed when the reference you hold goes out of scope.  There is no need to deregister a subscriber because all subscribers are held by the event bus as a weak reference.

4. Post a message onto the bus.
```c#
var rootEventBus = serviceProvider.GetService<IRootEventBus>();
rootEventBus.PublishAsync(new LogMessage("A message has been sent."));
```
## Subscribe Methods
```c#
TSubscriber Subscribe<TSubscriber>(bool subscribed = true) where TSubscriber : ISubscriber;
```
Attach a subscriber to the event bus, constructing the subscriber instance using dependency injection;
returns: The subscriber instance.

```c#
FilterSubscriber<TMessage> Subscribe<TMessage>(Func<EventContext, TMessage, Task> messageDelegate, bool subscribed = true) where TMessage : IMessage;
```
Subscribe using a method delegate or lambda function, filtering for certain types of message;
returns: the subscriber instance.

```c#
ISubscriber Subscribe(Func<EventContext, IMessage, Task> messageDelegate, bool subscribed = true);
```
Subscribe using a method delegate or lambda function, to messages of all types.  You should filter for types of messages by testing message is MessageClass or similar;
returns: the subscriber instance.

```c#
ISubscriber Subscribe<TMessage>(Func<Task> messageDelegate, bool subscribed = true) where TMessage: IMessage;
```
Subscribe using a method delegate or lambda function.  You do not receive the message instance, simply a notification that a message of th type TMessage was received.
returns: the subscriber instance.

```c#
ISubscriber Subscribe(ISubscriber subscriber);
```
Subscribe by passing an instance of ISubscriber.  You can implement your own instance or utilize FilterSubscriber or InjectSubscriber.
returns: the subscriber instance.
FilterSubscriber: A subscriber that takes a method delegate or lambda function and passes messages of a certain type on to it.
InjectSubscriber: A subscriber that, on receipt of a message, delegates to a subscriber created using dependency injection.

## Options

When using the default setup extensions, you can configure several options using the options action:
```c#
services.AddEventBus((options) => {
  options...
});
```

1. options.UseDefaultExceptionSubscriber() - Add a default exception subscriber to all event buses.
2. options.UseDefaultLoggingSubscriber() - Add the default handler for receipt of EventBusLogMessage messages.
3. options.AddExceptionPublisher() - Add an exception logger which automatically logs exceptions during event bus processing, by publishing EventBusExceptionMessage's back onto the event bus.
4. options.AddLoggingPublisher() - Log receipt of all messages by publishing an EventBusLogMessage back onto the event bus that received that message.
5. options.AddDefaultRootBus() - Register a singleton instance of RootEventBus as IRootEventBus.
6. options.AddDefaultClientBus() - Register a singleton instance of ClientEventBus as IClientEventBus, linked to the IRootEventBus which must also be registered.
7. options.SetDefaultExceptionMessage() - Set the default message for exception logging when no other message is available.


## Event Logging
Event logging can be enabled via the standard Microsoft.Extensions.Logging.Abstractions interface ILogger.  In order to enable logging, add calls to the following methods to your subclass of EventAggregator; or set appropriate setting when you call serviceCollection.AddEventBus()

* AddDefaultExceptionSubscriber() - Enable the generation of exception messages of type EventBusExceptionMessage, and wire up a subscriber that logs via logger.LogError.
* AddDefaultLoggingSubscriber() - Enable the generation of logging messages of type EventBusLogMessage, and wire up a subscriber that logs via logger.Log(LogLevel, Message).
* AddExceptionPublisher() - Enable the raising of EventBusExceptionMessage, but you will need to wire up your own custom subscriber.
* AddLoggingPublisher() - Enable the raising of EventBusLogMessage, but you will need to wire up your own custom subscriber.

Following, an example EventAggregator subclass that makes use of default logging:
```c#
public StateEventAggregator(IServiceProvider serviceProvider):base (serviceProvider)
{
  this.AddDefaultExceptionSubscriber();
  this.AddDefaultLogSubscriber();
}
```

## Bus Heirarchies
A ChildEventAggregator is a special instance of the bus that receives messages from its parent bus, and republishes all messages within the child.  Messages published on the child bus are not propagated back to the parent.

You can either create a general purpose child bus, or derive from ChildEventAggregator to create a specific bus and resolve it using dependency injection.
The CreateChildBus method provides a general bus that is a child to the one from which it was created.
```c#
IEventAggregator CreateChildBus();
```  

A great use-case from a child bus is to drive the user interface of a component section of an application, in concert with the application root bus.  For example, you could publish log messages into the Root bus (they will also be received by the ui child bus); and toast message events into the ui child bus exclusively for presentation in the user interface.  Because the event buses utilize a threading channel, you may need to marshal child subscribers to the ui thread at runtime.

## Support

Outrage.EventBus works in all forms of .Net Standard 2.1, and in all known frameworks, especially:
* Blazor Server - as a means of connecting otherwise disconnected components, such as toast components.
* Blazor Web Assembly - yes, it has the same support as BlazorServer.

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
