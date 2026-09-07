using BaseLib.Interfaces;
using ConsoleLib.Showcase.DesktopHost;
using ConsoleLib.Showcase.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ConsoleLib.Showcase.Tests;

[TestClass]
public sealed class ExtConShowcaseHostCapabilitiesTests
{
    [TestMethod]
    public void CapabilitiesAdvertiseNativeMouseAndTerminalSupport()
    {
        var console = Substitute.For<IConsole>();
        var capabilities = new ExtConShowcaseHostCapabilities(
            console,
            Substitute.For<IShowcaseTerminalService>());

        Assert.IsTrue(capabilities.SupportsMouse);
        Assert.IsTrue(capabilities.SupportsTerminal);
        Assert.IsNull(capabilities.Clipboard);
    }

    [TestMethod]
    public void AlertUsesConsoleBeepWhenOutputIsInteractive()
    {
        var console = Substitute.For<IConsole>();
        console.IsOutputRedirected.Returns(false);
        var alert = new ConsoleAlertService(console);

        alert.Alert();

        console.Received(1).Beep(880, 120);
    }

    [TestMethod]
    public void AlertIsUnavailableWhenOutputIsRedirected()
    {
        var console = Substitute.For<IConsole>();
        console.IsOutputRedirected.Returns(true);
        var alert = new ConsoleAlertService(console);

        Assert.IsFalse(alert.IsAvailable);
        alert.Alert();

        console.DidNotReceive().Beep(Arg.Any<int>(), Arg.Any<int>());
    }
}
