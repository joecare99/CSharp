using System;
using System.Threading;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>Schedules callbacks onto an application dispatcher.</summary>
public sealed class ApplicationScheduler : IScheduler
{
    private readonly IDispatcher _dispatcher;
    private readonly IClock _clock;
    private int _disposed;

    /// <summary>Initializes a scheduler with an application dispatcher and clock.</summary>
    public ApplicationScheduler(IDispatcher dispatcher, IClock clock)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <inheritdoc />
    public IDisposable Schedule(Action callback, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        if (callback == null) throw new ArgumentNullException(nameof(callback));
        if (delay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(delay));
        if (Volatile.Read(ref _disposed) != 0) throw new ObjectDisposedException(nameof(ApplicationScheduler));
        var handle = new ScheduleHandle(_dispatcher, callback, cancellationToken, delay, _clock);
        handle.Start();
        return handle;
    }

    /// <inheritdoc />
    public void Dispose() => Interlocked.Exchange(ref _disposed, 1);

    private sealed class ScheduleHandle : IDisposable
    {
        private readonly IDispatcher _dispatcher;
        private readonly Action _callback;
        private readonly CancellationToken _cancellationToken;
        private readonly IClock _clock;
        private Timer? _timer;
        private int _cancelled;

        internal ScheduleHandle(IDispatcher dispatcher, Action callback, CancellationToken cancellationToken, TimeSpan delay, IClock clock)
        {
            _dispatcher = dispatcher;
            _callback = callback;
            _cancellationToken = cancellationToken;
            _clock = clock;
            Delay = delay;
        }

        private TimeSpan Delay { get; }

        internal void Start()
        {
            if (_cancellationToken.IsCancellationRequested) return;
            _timer = new Timer(_ => Fire(), null, Delay, Timeout.InfiniteTimeSpan);
        }

        private void Fire()
        {
            if (Volatile.Read(ref _cancelled) != 0 || _cancellationToken.IsCancellationRequested) return;
            _ = _clock.UtcNow;
            _dispatcher.Dispatch(_callback, _cancellationToken);
            Dispose();
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _cancelled, 1) == 0)
            {
                _timer?.Dispose();
                _timer = null;
            }
        }
    }
}
