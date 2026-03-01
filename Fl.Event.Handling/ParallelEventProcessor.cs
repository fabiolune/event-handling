using LanguageExt;
using LanguageExt.Common;

namespace Fl.Event.Handling;

/// <summary>
/// An <see cref="IEventProcessor{T}"/> that dispatches an event to all registered
/// <see cref="IEventHandler{T}"/> instances concurrently.
/// </summary>
/// <typeparam name="T">The type of the event to process. Must be a reference type.</typeparam>
/// <param name="eventHandlers">The collection of handlers to invoke in parallel.</param>
public class ParallelEventProcessor<T>(IEnumerable<IEventHandler<T>> eventHandlers) : IEventProcessor<T> where T : class
{
    private readonly IEnumerable<IEventHandler<T>> _eventHandlers = eventHandlers;

    /// <summary>
    /// Asynchronously dispatches the specified event to all registered handlers in parallel,
    /// awaiting all of them before returning.
    /// </summary>
    /// <param name="evt">The event instance to process.</param>
    /// <returns>
    /// An <see cref="EitherAsync{Error, Unit}"/> that resolves to <see cref="Unit"/> when all
    /// handlers succeed, or an <see cref="Error"/> if any handler fails.
    /// </returns>
    public EitherAsync<Error, Unit> ProcessAsync(T evt) =>
        _eventHandlers
            .Select(handler => handler.HandleAsync(evt))
            .SequenceParallel()
            .Map(_ => Unit.Default);
}
