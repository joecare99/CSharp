using System;
using System.Collections.Generic;
using System.Threading;
using ConsoleLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class ApplicationServicesTests
{
    [TestMethod]
    public void Dispatcher_Preserves_Order_And_Queue_Is_Isolated()
    {
        var first = new ApplicationDispatcher(new ApplicationMessageQueue());
        var second = new ApplicationDispatcher(new ApplicationMessageQueue());
        var values = new List<int>();

        first.Dispatch(() => values.Add(1));
        first.Dispatch(() => values.Add(2));
        second.Dispatch(() => values.Add(3));

        Assert.AreEqual(2, first.ProcessPending());
        Assert.AreEqual(1, second.ProcessPending());
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, values);
    }

    [TestMethod]
    public void Dispatcher_Skips_Cancelled_Work_And_Continues_After_Error()
    {
        var dispatcher = new ApplicationDispatcher(new ApplicationMessageQueue());
        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        var cancelledRan = false;
        var normalRan = false;
        var errors = new List<Exception>();

        dispatcher.Dispatch(() => cancelledRan = true, cancelled.Token);
        dispatcher.Dispatch(() => throw new InvalidOperationException("expected"));
        dispatcher.Dispatch(() => normalRan = true);

        Assert.AreEqual(2, dispatcher.ProcessPending(errors.Add));
        Assert.IsFalse(cancelledRan);
        Assert.IsTrue(normalRan);
        Assert.AreEqual(1, errors.Count);
    }

    [TestMethod]
    public void Scheduler_Dispatches_Once_And_Can_Be_Cancelled()
    {
        var dispatcher = new ApplicationDispatcher(new ApplicationMessageQueue());
        using var scheduler = new ApplicationScheduler(dispatcher, new SystemClock());
        var count = 0;
        using var cancelled = new CancellationTokenSource();
        using (scheduler.Schedule(() => count++, TimeSpan.FromMilliseconds(10)))
        using (scheduler.Schedule(() => count++, TimeSpan.FromMilliseconds(10), cancelled.Token))
        {
            cancelled.Cancel();
            Thread.Sleep(50);
            Assert.AreEqual(1, dispatcher.ProcessPending());
        }

        Assert.AreEqual(1, count);
    }
}
