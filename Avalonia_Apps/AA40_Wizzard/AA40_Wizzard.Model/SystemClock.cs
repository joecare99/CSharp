using System;

namespace AA40_Wizzard.Model;

/// <summary>
/// Default implementation for current system time access.
/// </summary>
public sealed class SystemClock : ISystemClock
{
    /// <summary>
    /// Gets or sets the delegate used to obtain the current time.
    /// </summary>
    public static Func<DateTime> GetNow { get; set; } = static () => DateTime.Now;

    /// <inheritdoc />
    public DateTime Now => GetNow();

    /// <inheritdoc />
    public DateTime Today => GetNow().Date;
}
