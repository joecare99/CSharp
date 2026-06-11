using System;

namespace AA40_Wizzard.Model;

/// <summary>
/// Defines a minimal logging abstraction for the wizard model.
/// </summary>
public interface ILogSink
{
    /// <summary>
    /// Logs a message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Log(string message);

    /// <summary>
    /// Logs a message and related exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The related exception.</param>
    void Log(string message, Exception exception);
}
