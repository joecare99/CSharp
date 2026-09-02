using ConsoleLib;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ConsoleLibTests;

[TestClass]
public class ApplicationSchedulerCoverageTests
{
    private sealed class Dispatcher : IDispatcher
    {
        private readonly ConcurrentQueue<Action> _callbacks = new();

        public bool CheckAccess => true;
        public int DispatchCount { get; private set; }

        public void Dispatch(Action callback, CancellationToken cancellationToken = default)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                DispatchCount++;
                _callbacks.Enqueue(callback);
            }
        }

        public int ProcessPending(Action<Exception>? errorHandler = null)
        {
            var count = 0;
            while (_callbacks.TryDequeue(out var callback))
            {
                try
                {
                    callback();
                    count++;
                }
                catch (Exception exception) when (errorHandler is not null)
                {
                    errorHandler(exception);
                }
            }
            return count;
        }
    }

    private sealed class Clock : IClock
    {
        public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow;
    }

    [TestMethod]
    public void ConstructorAndSchedule_ValidateArgumentsAndDispatchCallback()
    {
        var dispatcher = new Dispatcher();
        var clock = new Clock();
        using var scheduler = new ApplicationScheduler(dispatcher, clock);
        var called = false;

        Assert.Throws<ArgumentNullException>(() => new ApplicationScheduler(null!, clock));
        Assert.Throws<ArgumentNullException>(() => new ApplicationScheduler(dispatcher, null!));
        Assert.Throws<ArgumentNullException>(() => scheduler.Schedule(null!, TimeSpan.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Schedule(() => { }, TimeSpan.FromMilliseconds(-1)));

        scheduler.Schedule(() => called = true, TimeSpan.Zero);
        Assert.IsFalse(called);
        SpinWait.SpinUntil(() => dispatcher.DispatchCount == 1, TimeSpan.FromSeconds(2));
        Assert.AreEqual(1, dispatcher.ProcessPending());
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void Schedule_CanBeCancelledBeforeTimerFires()
    {
        var dispatcher = new Dispatcher();
        using var scheduler = new ApplicationScheduler(dispatcher, new Clock());
        var called = false;
        var handle = scheduler.Schedule(() => called = true, TimeSpan.FromMilliseconds(100));

        handle.Dispose();
        Thread.Sleep(150);
        Assert.AreEqual(0, dispatcher.DispatchCount);
        Assert.IsFalse(called);
    }

    [TestMethod]
    public void Schedule_RespectsCancelledTokenAndDisposedScheduler()
    {
        var dispatcher = new Dispatcher();
        using var scheduler = new ApplicationScheduler(dispatcher, new Clock());
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        scheduler.Schedule(() => { }, TimeSpan.Zero, cancellation.Token);
        Thread.Sleep(50);
        Assert.AreEqual(0, dispatcher.DispatchCount);

        scheduler.Dispose();
        Assert.Throws<ObjectDisposedException>(() => scheduler.Schedule(() => { }, TimeSpan.Zero));
    }
}
