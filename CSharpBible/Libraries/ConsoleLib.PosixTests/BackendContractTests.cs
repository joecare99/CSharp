using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if NET5_0_OR_GREATER
using ConsoleLib.Posix;
#endif

namespace ConsoleLibTests;

[TestClass]
public sealed class BackendContractTests
{
#if NET5_0_OR_GREATER
    [TestMethod]
    public async Task AnsiOutputWriter_ClampsCursorAndWritesBackgroundColors()
    {
        await using var transport = new InMemoryTerminalTransport();
        await transport.OpenAsync();
        var output = new AnsiOutputWriter(transport);

        await output.MoveCursorAsync(0, 0);
        await output.SetBackgroundAsync(ConsoleColor.DarkBlue);
        await output.SetBackgroundAsync(ConsoleColor.Yellow);
        await output.SetForegroundAsync(ConsoleColor.DarkGray);
        await output.SetForegroundAsync(ConsoleColor.White);

        Assert.AreEqual("\u001b[1;1H\u001b[41m\u001b[106m\u001b[90m\u001b[97m", transport.Output);
    }

    [TestMethod]
    public async Task AnsiOutputWriter_ControlsMouseTracking()
    {
        await using var transport = new InMemoryTerminalTransport();
        await transport.OpenAsync();
        var output = new AnsiOutputWriter(transport);

        await output.EnableMouseTrackingAsync();
        await output.DisableMouseTrackingAsync();

        Assert.AreEqual(SgrMouseEncoder.EnableTracking + SgrMouseEncoder.DisableTracking, transport.Output);
    }

    [TestMethod]
    public void SgrMouseEncoder_EncodesButtonsWheelMoveAndModifiers()
    {
        var middle = new PointerInput(new Point(0, 0), PointerInputKind.Press, PointerButtons.Middle);
        var right = new PointerInput(new Point(1, 1), PointerInputKind.Press, PointerButtons.Right);
        var wheelDown = new PointerInput(new Point(-1, -1), PointerInputKind.Wheel, wheelDelta: -1);
        var move = new PointerInput(new Point(2, 3), PointerInputKind.Move, PointerButtons.Left,
            modifiers: KeyModifiers.Shift | KeyModifiers.Alt | KeyModifiers.Control);

        Assert.AreEqual("\u001b[<1;1;1M", SgrMouseEncoder.Encode(middle));
        Assert.AreEqual("\u001b[<2;2;2M", SgrMouseEncoder.Encode(right));
        Assert.AreEqual("\u001b[<65;1;1M", SgrMouseEncoder.Encode(wheelDown));
        Assert.AreEqual("\u001b[<60;3;4M", SgrMouseEncoder.Encode(move));
    }

    [TestMethod]
    public async Task PosixTransport_ReturnsNullWhenClosedAndClampsResize()
    {
        await using var transport = new PosixTerminalTransport(new MemoryStream(), new MemoryStream());

        Assert.IsFalse(transport.IsOpen);
        Assert.IsNull(await transport.ReadAsync());
        await transport.WriteAsync("ignored");
        await transport.ResizeAsync(new Size(0, -2));

        Assert.AreEqual(new Size(1, 1), transport.Size);
    }

    [TestMethod]
    public async Task PosixTransport_OpenIsIdempotentAndCloseIsSafe()
    {
        await using var transport = new PosixTerminalTransport(
            new MemoryStream(Encoding.UTF8.GetBytes("x")), new MemoryStream());

        await transport.OpenAsync();
        await transport.OpenAsync();
        await transport.CloseAsync();
        await transport.CloseAsync();

        Assert.IsFalse(transport.IsOpen);
    }

    [TestMethod]
    public async Task PosixTransport_CancellationIsObservedByOperations()
    {
        await using var transport = new PosixTerminalTransport(new MemoryStream(), new MemoryStream());
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        try
        {
            await transport.OpenAsync(cancellation.Token);
            Assert.Fail("OpenAsync should observe cancellation.");
        }
        catch (OperationCanceledException)
        {
        }

        try
        {
            await transport.ResizeAsync(new Size(2, 2), cancellation.Token);
            Assert.Fail("ResizeAsync should observe cancellation.");
        }
        catch (OperationCanceledException)
        {
        }

        try
        {
            await transport.CloseAsync(cancellation.Token);
            Assert.Fail("CloseAsync should observe cancellation.");
        }
        catch (OperationCanceledException)
        {
        }
    }
#endif
}
