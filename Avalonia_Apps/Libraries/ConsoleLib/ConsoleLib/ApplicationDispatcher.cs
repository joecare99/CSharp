using System;
using System.Threading;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>Default dispatcher backed by an application-scoped message queue.</summary>
public sealed class ApplicationDispatcher : IDispatcher
{
    private readonly IMessageQueue _queue;
    private readonly int _ownerThreadId;

    /// <summary>Initializes a dispatcher using the supplied queue.</summary>
    public ApplicationDispatcher(IMessageQueue queue)
    {
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _ownerThreadId = Thread.CurrentThread.ManagedThreadId;
    }

    /// <inheritdoc />
    public bool CheckAccess => Thread.CurrentThread.ManagedThreadId == _ownerThreadId;

    /// <inheritdoc />
    public void Dispatch(Action callback, CancellationToken cancellationToken = default)
    {
        _queue.Enqueue(callback, cancellationToken);
    }

    /// <inheritdoc />
    public int ProcessPending(Action<Exception>? errorHandler = null)
    {
        return _queue.ProcessPending(errorHandler);
    }
}
