# Fl.Event.Handling.Extensions

This library provides dependency injection extensions for registering the event handling components from [Fl.Event.Handling](../Fl.Event.Handling/README.md) into an `IServiceCollection`.

## Components

### `EventHandlingStrategy`

An enum that controls how an event is dispatched to its registered handlers.

| Value | Description |
|---|---|
| `Sequential` | Handlers are invoked one at a time, in registration order. Processing stops immediately if any handler returns an error. |
| `Parallel` | Handlers are invoked concurrently and all are awaited before the result is returned. |

---

### `ServiceCollectionExtensions`

Provides the `AddEventHandling<TEvent>` extension method on `IServiceCollection`.

```csharp
IServiceCollection AddEventHandling<TEvent>(
    this IServiceCollection services,
    EventHandlingStrategy strategy,
    Assembly assembly)
    where TEvent : class
```

This method:

1. Registers an `IEventProcessor<TEvent>` singleton — either `SequentialEventProcessor<TEvent>` or `ParallelEventProcessor<TEvent>` — depending on the chosen `strategy`.
2. Scans the provided `assembly` and registers all concrete `IEventHandler<TEvent>` implementations as singleton services.

An `ArgumentException` is thrown if `strategy` is not a valid `EventHandlingStrategy` value.

---

## Usage

```csharp
using Fl.Event.Handling.Extensions;
using System.Reflection;

// Inside your IServiceCollection configuration (e.g. Program.cs or a startup class):
services.AddEventHandling<MyEvent>(
    EventHandlingStrategy.Sequential,
    Assembly.GetExecutingAssembly());
```

To use the parallel strategy instead:

```csharp
services.AddEventHandling<MyEvent>(
    EventHandlingStrategy.Parallel,
    Assembly.GetExecutingAssembly());
```

Multiple event types can be registered independently:

```csharp
services.AddEventHandling<OrderPlacedEvent>(EventHandlingStrategy.Sequential, Assembly.GetExecutingAssembly());
services.AddEventHandling<PaymentReceivedEvent>(EventHandlingStrategy.Parallel, Assembly.GetExecutingAssembly());
```

Each call scans the assembly for the corresponding `IEventHandler<TEvent>` implementations and registers them alongside their processor.
