> This documentation is in line with the active development, hence should be considered work in progress. To check the documentation for the latest stable version please visit [https://fabiolune.github.io/event-handling/](https://fabiolune.github.io/event-handling/)

# Event Handling made simple

[![dotnet](https://github.com/fabiolune/event-handling/actions/workflows/main.yml/badge.svg)](https://github.com/fabiolune/event-handling/actions/workflows/main.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fabiolune_event-handling&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=fabiolune_event-handling)
[![codecov](https://codecov.io/gh/fabiolune/event-handling/graph/badge.svg?token=5F5UAWQ8K7)](https://codecov.io/gh/fabiolune/event-handling)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Ffabiolune%2Fevent-handling%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/fabiolune/event-handling/main)

A collection of lightweight, functional C# libraries for building event-driven pipelines. All processing results are expressed as `EitherAsync<Error, Unit>` (via [LanguageExt](https://github.com/louthy/language-ext)), keeping the pipelines composable and exception-free by design.

## Libraries

### [Fl.Event.Handling](Fl.Event.Handling/README.md)

The core library. Provides:

- **`IEventHandler<TEvent>`** — the unit of work: a single handler for a specific event type.
- **`IEventProcessor<TEvent>`** — the orchestrator: coordinates one or more handlers.
- **`SequentialEventProcessor<T>`** and **`ParallelEventProcessor<T>`** — built-in processor implementations.
- **`ExceptionCatcherEventProcessor<TEvent>`** and **`LoggingEventProcessor<TEvent>`** — decorators for cross-cutting concerns.

### [Fl.Event.Handling.Extensions](Fl.Event.Handling.Extensions/README.md)

Dependency injection extensions for ASP.NET Core / `IServiceCollection`. Provides `AddEventHandling<TEvent>()` to register a processor and auto-discover all `IEventHandler<TEvent>` implementations from an assembly in a single call.
