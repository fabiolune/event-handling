using LanguageExt;
using LanguageExt.Common;

namespace Fl.Event.Handling;

/// <summary>
/// Defines a processor that orchestrates the handling of events of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of the event to process. Must be a reference type.</typeparam>
public interface IEventProcessor<TEvent> where TEvent : class
{
    /// <summary>
    /// Asynchronously processes the specified event.
    /// </summary>
    /// <param name="evt">The event instance to process.</param>
    /// <returns>
    /// An <see cref="EitherAsync{Error, Unit}"/> that resolves to <see cref="Unit"/> on success,
    /// or an <see cref="Error"/> if processing fails.
    /// </returns>
    EitherAsync<Error, Unit> ProcessAsync(TEvent evt);
}