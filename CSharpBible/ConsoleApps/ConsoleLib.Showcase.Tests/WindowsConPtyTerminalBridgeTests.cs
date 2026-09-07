using ConsoleLib.Showcase.Terminal.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace ConsoleLib.Showcase.Tests;

[TestClass]
public sealed class WindowsConPtyTerminalBridgeTests
{
    [TestMethod]
    public void Factory_ReportsPlatformSupportWithoutStartingAProcess()
    {
        var factory = new WindowsConPtyTerminalSessionFactory();

        Assert.AreEqual(OperatingSystem.IsWindows(), factory.IsSupported);
        Assert.AreEqual(nameof(WindowsConPtyTerminalSessionFactory), factory.Name);
    }

    [TestMethod]
    public async Task Bridge_OnUnsupportedPlatform_RejectsStart()
    {
        if (OperatingSystem.IsWindows())
            Assert.Inconclusive("This contract test is only applicable on non-Windows hosts.");

        await using var bridge = new WindowsConPtyTerminalBridge();
        await Assert.ThrowsExactlyAsync<PlatformNotSupportedException>(
            () => bridge.StartAsync(new global::Terminal.Core.TerminalSessionOptions { FileName = "cmd.exe" }));
    }
}
