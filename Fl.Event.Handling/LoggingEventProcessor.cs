using LanguageExt;
using LanguageExt.Common;
using Serilog;
using Fl.Shared.Utils.Components.Logging.Extensions.Either;

namespace Fl.Event.Handling;

/// <summary>
/// An <see cref="IEventProcessor{TEvent}"/> decorator that adds structured logging around the
/// inner processor's execution using a Serilog <see cref="ILogger"/>.
/// </summary>
/// <typeparam name="TEvent">The type of the event to process. Must be a reference type.</typeparam>
/// <param name="processor">The inner <see cref="IEventProcessor{TEvent}"/> to wrap.</param>
/// <param name="logger">The Serilog logger used to record processing outcomes.</param>
public class LoggingEventProcessor<TEvent>(IEventProcessor<TEvent> processor, ILogger logger) 
    : IEventProcessor<TEvent> where TEvent : class
{
    private readonly IEventProcessor<TEvent> _processor = processor;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Asynchronously processes the specified event and logs the outcome via the configured
    /// <see cref="ILogger"/>.
    /// </summary>
    /// <param name="evt">The event instance to process.</param>
    /// <returns>
    /// An <see cref="EitherAsync{Error, Unit}"/> that resolves to <see cref="Unit"/> on success,
    /// or an <see cref="Error"/> if processing fails.
    /// </returns>
    public EitherAsync<Error, Unit> ProcessAsync(TEvent evt) => 
        _processor
            .ProcessAsync(evt)
            .TeeLog(_logger, nameof(IEventProcessor<TEvent>));
}
