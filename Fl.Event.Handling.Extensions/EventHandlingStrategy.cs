namespace Fl.Event.Handling.Extensions;

/// <summary>
/// Specifies the strategy used to dispatch events to their registered handlers.
/// </summary>
public enum EventHandlingStrategy
{
    /// <summary>
    /// Handlers are invoked one at a time, in registration order.
    /// Processing stops immediately if any handler returns an error.
    /// </summary>
    Sequential,

    /// <summary>
    /// Handlers are invoked concurrently and all are awaited before the result is returned.
    /// </summary>
    Parallel
}
