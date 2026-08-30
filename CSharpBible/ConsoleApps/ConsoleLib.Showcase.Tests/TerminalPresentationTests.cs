using ConsoleLib.Showcase.Terminal.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Terminal.Core;

namespace ConsoleLib.Showcase.Tests;

[TestClass]
public sealed class TerminalPresentationTests
{
    [TestMethod]
    public void SnapshotRenderer_PreservesViewportDimensions()
    {
        var document = new TerminalDocument(new TerminalSize(4, 2));
        document.ApplyOutput("AB");

        var rows = new TerminalSnapshotRenderer().Render(document.CreateSnapshot());

        Assert.AreEqual(2, rows.Count);
        Assert.AreEqual("AB  ", rows[0]);
        Assert.AreEqual("    ", rows[1]);
    }

    [TestMethod]
    public void InputRouter_EncodesCommonKeys()
    {
        var router = new TerminalInputRouter();

        Assert.AreEqual("\u001b[A", router.Encode(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false)));
        Assert.AreEqual("\r", router.Encode(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false)));
        Assert.AreEqual("x", router.Encode(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false)));
    }

    [TestMethod]
    public void MouseNegotiator_IsDisabledUntilSgrTrackingIsReported()
    {
        var document = new TerminalDocument(new TerminalSize(10, 2));
        var negotiator = new TerminalMouseNegotiator();

        Assert.IsFalse(negotiator.IsEnabled(document));
        Assert.IsNull(negotiator.Encode(document, 0, 1, 1, false));
    }
}
