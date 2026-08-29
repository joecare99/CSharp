using System;
using System.Text;
using System.Threading.Tasks;
using ConsoleLib;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if NET5_0_OR_GREATER
using ConsoleLib.Posix;
#endif

namespace ConsoleLibTests;

[TestClass]
public sealed class Osc52ClipboardServiceTests
{
    #if NET5_0_OR_GREATER
    [TestMethod]
    public async Task CopyAsync_WritesOsc52OnlyWhenEnabled()
    {
        var capabilities = new TerminalCapabilities(true, true, true, false, true, true, false, false);
        await using var transport = new InMemoryTerminalTransport(capabilities);
        await transport.OpenAsync();
        var service = new Osc52ClipboardService(
            transport,
            new ClipboardPolicy(allowOsc52Copy: true, maximumPayloadLength: 10));

        Assert.IsTrue(await service.CopyAsync("hi"));
        Assert.AreEqual("\u001b]52;c;" + Convert.ToBase64String(Encoding.UTF8.GetBytes("hi")) + "\a", transport.Output);
    }

    [TestMethod]
    public async Task PasteAsync_UsesFallbackWhenPolicyDisallowsOsc52()
    {
        var capabilities = new TerminalCapabilities(true, true, true, false, true, true, false, false);
        await using var transport = new InMemoryTerminalTransport(capabilities);
        await transport.OpenAsync();
        var service = new Osc52ClipboardService(
            transport,
            new ClipboardPolicy(),
            () => Task.FromResult<string?>("fallback"));

        Assert.AreEqual("fallback", await service.PasteAsync());
    }
    #endif
}
