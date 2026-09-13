using System;
using System.Threading;

namespace ConsoleLib.Interfaces;

/// <summary>
/// Represents an application-owned FIFO queue of callbacks.
/// </summary>
public interface IMessageQueue
{
    /// <summary>Gets the number of callbacks currently waiting in the queue.</summary>
    int Count { get; }

    /// <summary>Gets a signal that is set when a callback is added.</summary>
    WaitHandle Signal { get; }

    /// <summary>Adds a callback to the queue unless its token is already cancelled.</summary>
    void Enqueue(Action callback, CancellationToken cancellationToken = default);

    /// <summary>Executes queued callbacks in insertion order.</summary>
    /// <param name="errorHandler">Receives callback failures. A null handler rethrows the first failure.</param>
    /// <returns>The number of callbacks removed from the queue.</returns>
    int ProcessPending(Action<Exception>? errorHandler = null);
}
