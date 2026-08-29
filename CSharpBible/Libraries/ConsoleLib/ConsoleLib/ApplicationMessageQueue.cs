using System;
using System.Collections.Concurrent;
using System.Threading;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>
/// A thread-safe FIFO message queue owned by one application instance.
/// </summary>
public sealed class ApplicationMessageQueue : IMessageQueue, IDisposable
{
    private readonly ConcurrentQueue<QueuedCallback> _callbacks = new();
    private readonly AutoResetEvent _signal = new(false);
    private int _disposed;

    /// <inheritdoc />
    public int Count => _callbacks.Count;

    /// <inheritdoc />
    public WaitHandle Signal => _signal;

    /// <inheritdoc />
    public void Enqueue(Action callback, CancellationToken cancellationToken = default)
    {
        if (callback == null) throw new ArgumentNullException(nameof(callback));
        if (Volatile.Read(ref _disposed) != 0) throw new ObjectDisposedException(nameof(ApplicationMessageQueue));
        if (cancellationToken.IsCancellationRequested) return;
        _callbacks.Enqueue(new QueuedCallback(callback, cancellationToken));
        _signal.Set();
    }

    /// <inheritdoc />
    public int ProcessPending(Action<Exception>? errorHandler = null)
    {
        var processed = 0;
        while (_callbacks.TryDequeue(out var item))
        {
            processed++;
            if (item.CancellationToken.IsCancellationRequested) continue;
            try { item.Callback(); }
            catch (Exception error)
            {
                if (errorHandler == null) throw;
                errorHandler(error);
            }
        }
        return processed;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0) _signal.Dispose();
    }

    private readonly struct QueuedCallback
    {
        internal QueuedCallback(Action callback, CancellationToken cancellationToken)
        {
            Callback = callback;
            CancellationToken = cancellationToken;
        }

        internal Action Callback { get; }
        internal CancellationToken CancellationToken { get; }
    }
}
