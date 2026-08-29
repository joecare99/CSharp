using System.Threading.Tasks;
using ConsoleLib;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
#if NET5_0_OR_GREATER
using ConsoleLib.Posix;
#endif
using System;

namespace ConsoleLibTests;

[TestClass]
public sealed class PosixTerminalHostTests
{
#if NET5_0_OR_GREATER

    [TestMethod]
    public async Task Host_ManagesLifecycleAndDispatchesDecodedInput()
    {
        var capabilities = new TerminalCapabilities(true, true, true, true, false, false, false, false);
        await using var transport = new InMemoryTerminalTransport(capabilities);
        var host = new PosixTerminalHost(transport);
        var received = 0;
        host.KeyInputReceived += (_, _) => received++;

        await host.StartAsync();
        await host.ProcessInputAsync("A");
        await host.StopAsync();

        Assert.AreEqual(1, received);
        Assert.IsFalse(host.IsRunning);
        StringAssert.Contains(transport.Output, "\u001b[?1000h");
        StringAssert.Contains(transport.Output, "\u001b[?1000l");
    }

    [TestMethod]
    public async Task Host_RunAsync_ReadsTransportUntilEndOfStream()
    {
        await using var input = new MemoryStream(Encoding.UTF8.GetBytes("A"));
        await using var output = new MemoryStream();
        var transport = new PosixTerminalTransport(input, output,
            new TerminalCapabilities(true, true, false, false, false, false, false, false));
        await using var host = new PosixTerminalHost(transport);
        var received = 0;
        host.KeyInputReceived += (_, key) =>
        {
            if (key.Key == ConsoleKey.A)
                received++;
        };

        await host.RunAsync();

        Assert.AreEqual(1, received);
        Assert.IsFalse(host.IsRunning);
    }

    [TestMethod]
    public async Task Host_DispatchesKeyboardAndMouseFromMixedInput()
    {
        var capabilities = new TerminalCapabilities(true, true, true, true, false, false, false, false);
        await using var transport = new InMemoryTerminalTransport(capabilities);
        await using var host = new PosixTerminalHost(transport);
        var keys = 0;
        var pointers = 0;
        host.KeyInputReceived += (_, key) =>
        {
            if (key.Key == ConsoleKey.A || key.Key == ConsoleKey.B)
                keys++;
        };
        host.PointerInputReceived += (_, _) => pointers++;

        await host.StartAsync();
        await host.ProcessInputAsync("A\u001b[<0;4;3MB");

        Assert.AreEqual(2, keys);
        Assert.AreEqual(1, pointers);
    }

    [TestMethod]
    public async Task Host_DispatchesMixedInputWhenMouseSequenceIsSplitAcrossReads()
    {
        await using var transport = new InMemoryTerminalTransport();
        await using var host = new PosixTerminalHost(transport);
        var keys = 0;
        var pointers = 0;
        host.KeyInputReceived += (_, key) =>
        {
            if (key.Key == ConsoleKey.A || key.Key == ConsoleKey.B)
                keys++;
        };
        host.PointerInputReceived += (_, _) => pointers++;

        await host.StartAsync();
        await host.ProcessInputAsync("A\u001b[<");
        await host.ProcessInputAsync("0;4;3MB");

        Assert.AreEqual(2, keys);
        Assert.AreEqual(1, pointers);
    }
#endif
}
