using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ConsoleLib.Tests;

[TestClass]
public class V2CapabilityTests
{
    [TestMethod]
    public void PlainTextCapabilities_DisableInteractiveFeatures()
    {
        var capabilities = TerminalCapabilities.PlainText();

        Assert.IsFalse(capabilities.SupportsAnsi);
        Assert.IsFalse(capabilities.SupportsMouse);
        Assert.IsFalse(capabilities.SupportsClipboardCopy);
        Assert.IsFalse(capabilities.SupportsClipboardPaste);
    }

    [TestMethod]
    public void ClipboardPolicy_RequiresOptInCapabilityAndPayloadLimit()
    {
        var policy = new ClipboardPolicy(allowOsc52Copy: true, maximumPayloadLength: 3);
        var capabilities = new TerminalCapabilities(true, true, true, true, true, false, false, false);

        Assert.IsTrue(policy.CanCopy(capabilities, "abc"));
        Assert.IsFalse(policy.CanCopy(capabilities, "abcd"));
        Assert.IsFalse(policy.CanPaste(capabilities));
    }

    [TestMethod]
    public void ClipboardPolicy_RejectsInvalidPayloadLimit()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ClipboardPolicy(maximumPayloadLength: 0));
    }
}
