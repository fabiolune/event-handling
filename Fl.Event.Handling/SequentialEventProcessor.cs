using LanguageExt;
using LanguageExt.Common;

namespace Fl.Event.Handling;

/// <summary>
/// An <see cref="IEventProcessor{T}"/> that dispatches an event to all registered
/// <see cref="IEventHandler{T}"/> instances one at a time, in order.
/// </summary>
/// <typeparam name="T">The type of the event to process. Must be a reference type.</typeparam>
/// <param name="eventHandlers">The ordered collection of handlers to invoke sequentially.</param>
public class SequentialEventProcessor<T>(IEnumerable<IEventHandler<T>> eventHandlers) : IEventProcessor<T> where T : class
{
    private readonly IEnumerable<IEventHandler<T>> _eventHandlers = eventHandlers;

    /// <summary>
    /// Asynchronously dispatches the specified event to all registered handlers one at a time,
    /// stopping early if any handler returns an <see cref="Error"/>.
    /// </summary>
    /// <param name="evt">The event instance to process.</param>
    /// <returns>
    /// An <see cref="EitherAsync{Error, Unit}"/> that resolves to <see cref="Unit"/> when all
    /// handlers succeed, or an <see cref="Error"/> if any handler fails.
    /// </returns>
    public EitherAsync<Error, Unit> ProcessAsync(T evt) =>
        _eventHandlers
            .Select(handler => handler.HandleAsync(evt))
            .SequenceSerial()
            .Map(_ => Unit.Default);
}
