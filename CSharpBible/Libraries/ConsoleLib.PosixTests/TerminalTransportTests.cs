using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLibTests;

[TestClass]
public sealed class TerminalTransportTests
{
    [TestMethod]
    public async Task InMemoryTransportCapturesAnsiOutput()
    {
        await using var transport = new InMemoryTerminalTransport();
        await transport.OpenAsync();
        var output = new ConsoleLib.Posix.AnsiOutputWriter(transport);

        await output.ClearAsync();
        await output.MoveCursorAsync(3, 2);
        await output.SetForegroundAsync(ConsoleColor.Green);
        await output.WriteAsync("ok");
        await output.ResetAsync();

        Assert.AreEqual("\u001b[2J\u001b[H\u001b[2;3H\u001b[92mok\u001b[0m", transport.Output);
    }

    [TestMethod]
    public void CapabilityDetectionHonorsRedirectAndNoColor()
    {
        var capabilities = TerminalCapabilitiesDetector.Detect(
            inputRedirected: false,
            outputRedirected: false,
            environment: name => name == "TERM" ? "xterm-256color" : name == "NO_COLOR" ? "1" : null);

        Assert.IsFalse(capabilities.SupportsAnsi);
        Assert.IsFalse(capabilities.SupportsColor);
    }

    [TestMethod]
    public async Task PosixTransport_UsesRawModeLifecycle_AndReadsWritesStreams()
    {
        await using var input = new MemoryStream(Encoding.UTF8.GetBytes("in"));
        await using var output = new MemoryStream();
        var mode = new RecordingTerminalMode();
        await using var transport = new ConsoleLib.Posix.PosixTerminalTransport(input, output, terminalMode: mode);

        await transport.OpenAsync();
        Assert.IsTrue(transport.IsOpen);
        Assert.AreEqual("in", await transport.ReadAsync());
        await transport.WriteAsync("out");
        await transport.CloseAsync();

        Assert.AreEqual(1, mode.EnterCount);
        Assert.AreEqual(1, mode.RestoreCount);
        Assert.AreEqual("out", Encoding.UTF8.GetString(output.ToArray()));
        Assert.IsFalse(transport.IsOpen);
    }

    private sealed class RecordingTerminalMode : ConsoleLib.Posix.IPosixTerminalMode
    {
        public int EnterCount { get; private set; }
        public int RestoreCount { get; private set; }

        public Task EnterRawModeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnterCount++;
            return Task.CompletedTask;
        }

        public Task RestoreAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RestoreCount++;
            return Task.CompletedTask;
        }
    }
}
