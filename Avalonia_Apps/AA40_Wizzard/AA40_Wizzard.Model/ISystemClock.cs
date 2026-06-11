using System;

namespace AA40_Wizzard.Model;

/// <summary>
/// Provides the current system time.
/// </summary>
public interface ISystemClock
{
    /// <summary>
    /// Gets the current local time.
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Gets the current local date.
    /// </summary>
    DateTime Today { get; }
}
