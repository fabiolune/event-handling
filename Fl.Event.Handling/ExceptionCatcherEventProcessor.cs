using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.Common;

namespace Fl.Event.Handling;

/// <summary>
/// An <see cref="IEventProcessor{TEvent}"/> decorator that catches any unhandled exceptions thrown
/// by the inner processor and converts them into <see cref="Error"/> values.
/// </summary>
/// <typeparam name="TEvent">The type of the event to process. Must be a reference type.</typeparam>
/// <param name="processor">The inner <see cref="IEventProcessor{TEvent}"/> to wrap.</param>
public class ExceptionCatcherEventProcessor<TEvent>(IEventProcessor<TEvent> processor) 
    : IEventProcessor<TEvent> where TEvent : class
{
    private readonly IEventProcessor<TEvent> _processor = processor;

    /// <summary>
    /// Asynchronously processes the specified event, catching any exceptions thrown by the
    /// inner processor and returning them as <see cref="Error"/> values.
    /// </summary>
    /// <param name="evt">The event instance to process.</param>
    /// <returns>
    /// An <see cref="EitherAsync{Error, Unit}"/> that resolves to <see cref="Unit"/> on success,
    /// or an <see cref="Error"/> wrapping the caught exception if processing fails.
    /// </returns>
    public EitherAsync<Error, Unit> ProcessAsync(TEvent evt) =>
        Try(() => _processor.ProcessAsync(evt))
            .Match(e => e, ex => Error.New(ex));
}
