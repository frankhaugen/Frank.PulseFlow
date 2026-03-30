using Microsoft.Extensions.Logging;

namespace Frank.PulseFlow.Logging;

/// <summary>
/// Represents a log pulse.
/// </summary>
public sealed class LogPulse : BasePulse
{
    /// <summary>
    /// Gets the log level of the application.
    /// </summary>
    /// <value>The log level.</value>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Gets the unique identifier of an event.
    /// </summary>
    public EventId EventId { get; }

    /// <summary>
    /// Gets the exception associated with the property.
    /// </summary>
    /// <value>
    /// The exception associated with the property, or null if no exception occurred.
    /// </value>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets the name of the category.
    /// </summary>
    /// <value>The name of the category.</value>
    public string CategoryName { get; }

    /// <summary>
    /// Gets the message associated with this property.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the state of the object.
    /// </summary>
    /// <remarks>
    /// The state is represented as a collection of key-value pairs, where the key is a string and the value is an object.
    /// The state is read-only and can be null if there is no state available.
    /// </remarks>
    /// <returns>A read-only list of key-value pairs representing the state of the object.</returns>
    public IReadOnlyList<KeyValuePair<string, object?>>? State { get; }

    /// <summary>
    /// Gets a snapshot of active logging scopes (outer to inner) when the pulse was emitted, if any.
    /// </summary>
    public IReadOnlyList<object?>? Scope { get; }

    /// <summary>
    /// Initializes a log pulse with no scope snapshot (use the seven-parameter overload to include scopes).
    /// </summary>
    /// <param name="logLevel">The level of the log event.</param>
    /// <param name="eventId">The identifier of the log event.</param>
    /// <param name="exception">The exception associated with the log event, if any.</param>
    /// <param name="categoryName">The name of the log category.</param>
    /// <param name="message">The log message.</param>
    /// <param name="state">Structured log state, when provided by the logging framework.</param>
    public LogPulse(
        LogLevel logLevel,
        EventId eventId,
        Exception? exception,
        string categoryName,
        string message,
        IReadOnlyList<KeyValuePair<string, object?>>? state)
        : this(logLevel, eventId, exception, categoryName, message, state, null)
    {
    }

    /// <summary>
    /// Represents a log pulse, which encapsulates information about a log event.
    /// </summary>
    /// <param name="logLevel">The level of the log event.</param>
    /// <param name="eventId">The identifier of the log event.</param>
    /// <param name="exception">The exception associated with the log event, if any.</param>
    /// <param name="categoryName">The name of the log category.</param>
    /// <param name="message">The log message.</param>
    /// <param name="state">Structured log state, when provided by the logging framework.</param>
    /// <param name="scope">Optional scope chain from <see cref="Microsoft.Extensions.Logging.ILogger.BeginScope{TState}"/> (outer to inner).</param>
    public LogPulse(
        LogLevel logLevel,
        EventId eventId,
        Exception? exception,
        string categoryName,
        string message,
        IReadOnlyList<KeyValuePair<string, object?>>? state,
        IReadOnlyList<object?>? scope)
    {
        LogLevel = logLevel;
        EventId = eventId;
        Exception = exception;
        CategoryName = categoryName;
        Message = message;
        State = state;
        Scope = scope;
    }

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    /// <returns>
    /// A string representing the object. The string consists of the log level,
    /// event ID, category name, message, and exception, formatted in the following way:
    /// [LogLevel] (EventId) CategoryName: 'Message'
    /// Exception
    /// </returns>
    public override string ToString() =>
        Exception is null
            ? $"[{LogLevel}] ({EventId}) {CategoryName}: '{Message}'"
            : $"[{LogLevel}] ({EventId}) {CategoryName}: '{Message}'\n\t{Exception}";

}
