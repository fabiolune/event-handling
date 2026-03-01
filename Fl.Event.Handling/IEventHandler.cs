using LanguageExt;
using LanguageExt.Common;

namespace Fl.Event.Handling;

/// <summary>
/// Defines a handler responsible for processing a single event of type <typeparamref name="TEvent"/>.
/// </summary>
/// <typeparam name="TEvent">The type of the event to handle. Must be a reference type.</typeparam>
public interface IEventHandler<TEvent> where TEvent : class
{
    /// <summary>
    /// Asynchronously handles the specified event.
    /// </summary>
    /// <param name="evt">The event instance to handle.</param>
    /// <returns>
    /// An <see cref="EitherAsync{Error, Unit}"/> that resolves to <see cref="Unit"/> on success,
    /// or an <see cref="Error"/> if handling fails.
    /// </returns>
    EitherAsync<Error, Unit> HandleAsync(TEvent evt);
}