using AA98.Terminal.Host.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Tests;

[TestClass]
public class TerminalHostAppStartupTests
{
    [TestMethod]
    public void CreateServiceProvider_RegistersTerminalHostServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<AA98_AvlnCodeStudio.Base.OS.Services.ITerminalShellResolver, DevelopmentTerminalShellResolver>();
        services.AddSingleton<IHostedTerminalProcessFactory, TestHostedTerminalProcessFactory>();
        services.AddSingleton<AA98_AvlnCodeStudio.Base.OS.Services.ITerminalSessionService, HostedTerminalSessionService>();

        using var serviceProvider = services.BuildServiceProvider();

        Assert.IsNotNull(serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.OS.Services.ITerminalShellResolver>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<IHostedTerminalProcessFactory>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.OS.Services.ITerminalSessionService>());
    }

    [TestMethod]
    public async Task DevelopmentTerminalShellResolver_ShouldPreferExplicitShell()
    {
        var sut = new DevelopmentTerminalShellResolver();
        var request = new AA98_AvlnCodeStudio.Base.OS.Models.TerminalSessionStartRequest
        {
            ShellPath = "/custom/shell",
            ShellDisplayName = "custom-shell",
        };

        var shell = await sut.ResolveShellAsync(request);

        Assert.AreEqual("custom-shell", shell.DisplayName);
        Assert.AreEqual("/custom/shell", shell.ExecutablePath);
        Assert.IsFalse(shell.IsFallback);
    }

    [TestMethod]
    public async Task DevelopmentTerminalShellResolver_ShouldPreserveExplicitLinuxArguments()
    {
        var sut = new DevelopmentTerminalShellResolver();
        var request = new AA98_AvlnCodeStudio.Base.OS.Models.TerminalSessionStartRequest
        {
            ShellPath = "/bin/bash",
            ShellDisplayName = "bash",
        };
        request.Arguments.Add("-l");
        request.Arguments.Add("-i");

        var shell = await sut.ResolveShellAsync(request);

        CollectionAssert.AreEqual(new[] { "-l", "-i" }, shell.Arguments.ToArray());
        Assert.AreEqual("bash", shell.DisplayName);
        Assert.AreEqual("/bin/bash", shell.ExecutablePath);
        Assert.IsFalse(shell.IsFallback);
    }

    [TestMethod]
    public async Task DevelopmentTerminalShellResolver_ShouldFallbackWhenNoShellProvided()
    {
        var sut = new DevelopmentTerminalShellResolver();

        var shell = await sut.ResolveShellAsync(new AA98_AvlnCodeStudio.Base.OS.Models.TerminalSessionStartRequest());

        Assert.IsFalse(string.IsNullOrWhiteSpace(shell.ExecutablePath));
        Assert.IsTrue(shell.IsFallback);
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            StringAssert.StartsWith(shell.ExecutablePath, "/bin/");
        }
    }
}