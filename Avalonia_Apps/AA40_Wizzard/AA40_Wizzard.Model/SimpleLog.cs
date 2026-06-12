using System;
using System.Diagnostics;
using System.Globalization;

namespace AA40_Wizzard.Model;

/// <summary>
/// Writes timestamped log messages to a configurable sink.
/// </summary>
public sealed class SimpleLog(ISystemClock systemClock) : ILogSink
{
    /// <summary>
    /// Gets or sets the log sink action used by the application and tests.
    /// </summary>
    public static Action<string> LogAction { get; set; } = static message => Debug.WriteLine(message);

    private readonly ISystemClock _systemClock = systemClock;

    /// <inheritdoc />
    public void Log(string message)
        => LogAction($"{_systemClock.Now.ToString(CultureInfo.InvariantCulture)}: Msg: {message}");

    /// <inheritdoc />
    public void Log(string message, Exception exception)
        => LogAction($"{_systemClock.Now.ToString(CultureInfo.InvariantCulture)}: Err: {message}, {exception}");
}
