using System;
using System.Threading;

namespace ConsoleLib.Interfaces;

/// <summary>Schedules callbacks for dispatch after a delay.</summary>
public interface IScheduler : IDisposable
{
    /// <summary>Schedules a callback and returns a handle that can cancel it.</summary>
    IDisposable Schedule(Action callback, TimeSpan delay, CancellationToken cancellationToken = default);
}
