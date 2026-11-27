using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.OS.Models;
using AA98_AvlnCodeStudio.Base.OS.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98.Terminal.Host.Tests;

[TestClass]
public class NullTerminalSessionTests
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void OutputEvents_DoNotRetainSubscribers(int eventIndex)
    {
        var session = new NullTerminalSession(new TerminalShellDescriptor());
        var subscriber = Subscribe(session, eventIndex);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.IsFalse(subscriber.IsAlive);
        GC.KeepAlive(session);
    }

    [TestMethod]
    public async Task Session_DoesNotProduceOutputAndStillRaisesExit()
    {
        await using var session = new NullTerminalSession(new TerminalShellDescriptor());
        var outputCount = 0;
        var exitCount = 0;
        EventHandler<string> outputHandler = (_, _) => outputCount++;
        session.OutputReceived += outputHandler;
        session.OutputPartialReceived += outputHandler;
        session.ErrorReceived += outputHandler;
        session.ErrorPartialReceived += outputHandler;
        session.Exited += (_, code) =>
        {
            Assert.AreEqual(0, code);
            exitCount++;
        };

        session.MarkStarted();
        await session.WriteInputAsync("ignored");
        await session.StopAsync();
        await session.StopAsync();
        session.OutputReceived -= outputHandler;
        session.OutputPartialReceived -= outputHandler;
        session.ErrorReceived -= outputHandler;
        session.ErrorPartialReceived -= outputHandler;

        Assert.AreEqual(0, outputCount);
        Assert.AreEqual(1, exitCount);
        Assert.AreEqual(TerminalSessionState.Stopped, session.State);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference Subscribe(NullTerminalSession session, int eventIndex)
    {
        var observer = new OutputObserver();
        switch (eventIndex)
        {
            case 0:
                session.OutputReceived += observer.OnOutput;
                break;
            case 1:
                session.OutputPartialReceived += observer.OnOutput;
                break;
            case 2:
                session.ErrorReceived += observer.OnOutput;
                break;
            case 3:
                session.ErrorPartialReceived += observer.OnOutput;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eventIndex));
        }

        return new WeakReference(observer);
    }

    private sealed class OutputObserver
    {
        public void OnOutput(object? sender, string text)
        {
        }
    }
}
