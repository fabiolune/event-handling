using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Fl.Functional.Utils;
using System.Reflection;
namespace Fl.Event.Handling.Extensions;

/// <summary>
/// Provides extension methods for registering event handling services with the
/// <see cref="IServiceCollection"/> dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="IEventProcessor{TEvent}"/> and all <see cref="IEventHandler{TEvent}"/>
    /// implementations found in the specified <paramref name="assembly"/> with the DI container.
    /// </summary>
    /// <typeparam name="TEvent">The event type to configure handling for. Must be a reference type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="strategy">
    /// The <see cref="EventHandlingStrategy"/> that determines whether handlers are invoked
    /// sequentially or in parallel.
    /// </param>
    /// <param name="assembly">
    /// The <see cref="Assembly"/> to scan for <see cref="IEventHandler{TEvent}"/> implementations.
    /// </param>
    /// <returns>The original <paramref name="services"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="strategy"/> is not a recognised <see cref="EventHandlingStrategy"/> value.
    /// </exception>
    public static IServiceCollection AddEventHandling<TEvent>(this IServiceCollection services, EventHandlingStrategy strategy, Assembly assembly) where TEvent : class =>
        strategy
            .MakeOption(s => s is not EventHandlingStrategy.Sequential and not EventHandlingStrategy.Parallel)
            .Map(s => 
                s == EventHandlingStrategy.Sequential
                    ? services.Tee(s => s.TryAddSingleton<IEventProcessor<TEvent>, SequentialEventProcessor<TEvent>>())
                    : services.Tee(s => s.TryAddSingleton<IEventProcessor<TEvent>, ParallelEventProcessor<TEvent>>()))
            .IfNone(() => throw new ArgumentException($"Invalid event handling {nameof(strategy)}: '{strategy}'"))
            .Scan(scan => 
                scan
                    .FromAssemblies(assembly)
                    .AddClasses(cl => cl.AssignableTo(typeof(IEventHandler<TEvent>)))
                    .As<IEventHandler<TEvent>>()
                    .WithSingletonLifetime());
}
