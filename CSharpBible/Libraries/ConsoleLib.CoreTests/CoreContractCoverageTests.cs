using ConsoleLib;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLibTests;

[TestClass]
public class CoreContractCoverageTests
{
    [TestMethod]
    public async Task InMemoryTransport_OpenWriteResizeReadAndClose()
    {
        var transport = new InMemoryTerminalTransport();

        Assert.IsFalse(transport.IsOpen);
        Assert.AreEqual(new Size(80, 25), transport.Size);
        await transport.OpenAsync();
        await transport.WriteAsync("hello");
        await transport.WriteAsync(string.Empty);
        Assert.IsTrue(transport.IsOpen);
        Assert.AreEqual("hello", transport.Output);
        Assert.IsNull(await transport.ReadAsync());

        await transport.ResizeAsync(new Size(0, -2));
        Assert.AreEqual(new Size(1, 1), transport.Size);
        await transport.CloseAsync();
        Assert.IsFalse(transport.IsOpen);
        await transport.WriteAsync("ignored");
        Assert.AreEqual("hello", transport.Output);
        await transport.DisposeAsync();
    }

    [TestMethod]
    public async Task InMemoryTransport_CancellationIsHonored()
    {
        var transport = new InMemoryTerminalTransport();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        Assert.Throws<OperationCanceledException>(() => transport.OpenAsync(cancellation.Token));
        Assert.Throws<OperationCanceledException>(() => transport.ReadAsync(cancellation.Token));
        Assert.Throws<OperationCanceledException>(() => transport.WriteAsync("x", cancellation.Token));
        Assert.Throws<OperationCanceledException>(() => transport.ResizeAsync(new Size(2, 2), cancellation.Token));
        Assert.Throws<OperationCanceledException>(() => transport.CloseAsync(cancellation.Token));
    }

    [TestMethod]
    public void UnicodeTextLayoutService_HandlesEmptyControlCombiningAndWideText()
    {
        var service = new UnicodeTextLayoutService();

        Assert.AreEqual(0, service.GetCellWidth((string?)null));
        Assert.AreEqual(0, service.GetCellWidth(""));
        Assert.AreEqual(0, service.GetCellWidth('\n'));
        Assert.AreEqual(0, service.GetCellWidth('\u0301'));
        Assert.AreEqual(1, service.GetCellWidth('A'));
        Assert.AreEqual(2, service.GetCellWidth('界'));
        Assert.AreEqual(3, service.GetCellWidth("A界\u0301"));
    }

    [TestMethod]
    public void TerminalCapabilities_PlainTextSetsRedirectAndFeatureFlags()
    {
        var capabilities = TerminalCapabilities.PlainText(true, false);

        Assert.IsFalse(capabilities.SupportsAnsi);
        Assert.IsFalse(capabilities.SupportsColor);
        Assert.IsFalse(capabilities.SupportsCursor);
        Assert.IsFalse(capabilities.SupportsMouse);
        Assert.IsFalse(capabilities.SupportsClipboardCopy);
        Assert.IsFalse(capabilities.SupportsClipboardPaste);
        Assert.IsTrue(capabilities.IsInputRedirected);
        Assert.IsFalse(capabilities.IsOutputRedirected);
    }

    [TestMethod]
    public void TerminalCapabilitiesDetector_RecognizesAnsiColorAndMouse()
    {
        var capabilities = TerminalCapabilitiesDetector.Detect(false, false, name => name switch
        {
            "TERM" => "xterm-256color",
            "TERM_PROGRAM" => "Visual Studio Code",
            _ => null
        });

        Assert.IsTrue(capabilities.SupportsAnsi);
        Assert.IsTrue(capabilities.SupportsColor);
        Assert.IsTrue(capabilities.SupportsCursor);
        Assert.IsTrue(capabilities.SupportsMouse);
    }

    [TestMethod]
    public void TerminalCapabilitiesDetector_DisablesAnsiForRedirectedOrNoColorOrDumb()
    {
        static string? Dumb(string name) => name == "TERM" ? "dumb" : null;
        static string? NoColor(string name) => name == "TERM" ? "xterm-color" : name == "NO_COLOR" ? "1" : null;

        Assert.IsFalse(TerminalCapabilitiesDetector.Detect(false, true, _ => "xterm-color").SupportsAnsi);
        Assert.IsFalse(TerminalCapabilitiesDetector.Detect(false, false, Dumb).SupportsAnsi);
        Assert.IsFalse(TerminalCapabilitiesDetector.Detect(false, false, NoColor).SupportsAnsi);
        Assert.IsFalse(TerminalCapabilitiesDetector.Detect(false, false, _ => null).SupportsAnsi);
    }

    [TestMethod]
    public void ClipboardPolicy_EnforcesCapabilitiesPermissionAndMaximumLength()
    {
        var capable = new TerminalCapabilities(false, false, false, false, true, true, false, false);
        var incapable = TerminalCapabilities.PlainText();
        var policy = new ClipboardPolicy(allowOsc52Copy: true, allowOsc52Paste: true, maximumPayloadLength: 3);

        Assert.IsTrue(policy.CanCopy(capable, "abc"));
        Assert.IsFalse(policy.CanCopy(capable, "abcd"));
        Assert.IsFalse(policy.CanCopy(capable, ""));
        Assert.IsFalse(policy.CanCopy(capable, null));
        Assert.IsFalse(policy.CanCopy(incapable, "abc"));
        Assert.IsFalse(policy.CanCopy(null, "abc"));
        Assert.IsTrue(policy.CanPaste(capable));
        Assert.IsFalse(policy.CanPaste(incapable));
        Assert.IsFalse(policy.CanPaste(null));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ClipboardPolicy(maximumPayloadLength: 0));
    }

    [TestMethod]
    public void InputContracts_ExposeConstructorValuesAndDispatchState()
    {
        var key = new KeyInput(ConsoleKey.Enter, '\r', KeyModifiers.Control, true, true);
        var pointer = new PointerInput(new Point(2, 3), PointerInputKind.Wheel, PointerButtons.Left, 120, KeyModifiers.Shift);
        var context = new InputDispatchContext { Handled = false, StopPropagation = false };

        Assert.AreEqual(ConsoleKey.Enter, key.Key);
        Assert.AreEqual('\r', key.KeyChar);
        Assert.AreEqual(KeyModifiers.Control, key.Modifiers);
        Assert.IsTrue(key.IsKeyDown);
        Assert.IsTrue(key.IsRepeat);
        Assert.AreEqual(new Point(2, 3), pointer.Position);
        Assert.AreEqual(PointerInputKind.Wheel, pointer.Kind);
        Assert.AreEqual(PointerButtons.Left, pointer.Buttons);
        Assert.AreEqual(120, pointer.WheelDelta);
        Assert.AreEqual(KeyModifiers.Shift, pointer.Modifiers);
        context.MarkHandled(false);
        Assert.IsTrue(context.Handled);
        Assert.IsFalse(context.StopPropagation);
        context.MarkHandled();
        Assert.IsTrue(context.StopPropagation);
    }

    [TestMethod]
    public void TerminalCell_EqualityUsesAllCellValues()
    {
        var cell = new TerminalCell('X', ConsoleColor.White, ConsoleColor.Black);
        var equal = new TerminalCell('X', ConsoleColor.White, ConsoleColor.Black);
        var different = new TerminalCell('X', ConsoleColor.Yellow, ConsoleColor.Black);

        Assert.IsTrue(cell.Equals(equal));
        Assert.IsTrue(cell.Equals((object)equal));
        Assert.IsFalse(cell.Equals(different));
        Assert.IsFalse(cell.Equals("X"));
        Assert.AreEqual(cell.GetHashCode(), equal.GetHashCode());
    }

    [TestMethod]
    public void SystemClock_ReturnsUtcValue()
    {
        var before = DateTimeOffset.UtcNow;
        var value = new SystemClock().UtcNow;
        var after = DateTimeOffset.UtcNow;

        Assert.IsTrue(value >= before && value <= after);
    }
}
