using System;
using System.Threading;

namespace ConsoleLib.Interfaces;

/// <summary>Dispatches work to an application-owned message queue.</summary>
public interface IDispatcher
{
    /// <summary>Gets whether the caller is the dispatcher's owning thread.</summary>
    bool CheckAccess { get; }

    /// <summary>Posts work for execution by <see cref="ProcessPending"/>.</summary>
    void Dispatch(Action callback, CancellationToken cancellationToken = default);

    /// <summary>Executes all currently queued work.</summary>
    int ProcessPending(Action<Exception>? errorHandler = null);
}
