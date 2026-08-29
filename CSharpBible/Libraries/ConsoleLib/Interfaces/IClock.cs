using System;

namespace ConsoleLib.Interfaces;

/// <summary>Provides the current time to application services.</summary>
public interface IClock
{
    /// <summary>Gets the current UTC time.</summary>
    DateTimeOffset UtcNow { get; }
}
