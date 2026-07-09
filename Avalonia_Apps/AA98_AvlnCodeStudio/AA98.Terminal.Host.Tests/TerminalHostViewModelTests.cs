using AA98.Terminal.Host.Services;
using AA98_AvlnCodeStudio.Base.OS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Tests;

[TestClass]
public class TerminalHostServiceTests
{
    [TestMethod]
    public async Task HostedTerminalSessionService_ShouldResolveShellAndCreateSession()
    {
        var shellResolver = new DevelopmentTerminalShellResolver();
        var processFactory = new TestHostedTerminalProcessFactory();
        var sut = new HostedTerminalSessionService(shellResolver, processFactory);
        var request = new TerminalSessionStartRequest
        {
            ShellPath = "cmd.exe",
            ShellDisplayName = "cmd",
        };

        var session = await sut.StartSessionAsync(request);

        Assert.AreEqual(TerminalSessionState.Running, session.State);
        Assert.AreEqual("cmd", session.Shell.DisplayName);
        Assert.AreEqual("cmd.exe", session.Shell.ExecutablePath);
        Assert.AreEqual(1, processFactory.Requests.Count);
        await session.DisposeAsync();
    }

    [TestMethod]
    public async Task HostedTerminalSessionService_ShouldForwardConfigurationToProcessFactory()
    {
        var shellResolver = new DevelopmentTerminalShellResolver();
        var processFactory = new TestHostedTerminalProcessFactory();
        var sut = new HostedTerminalSessionService(shellResolver, processFactory);
        var request = new TerminalSessionStartRequest
        {
            ShellPath = "/bin/bash",
            ShellDisplayName = "bash",
            WorkingDirectory = "/workspace/aa98",
            WorkspaceRootPath = "/workspace",
        };
        request.Arguments.Add("-l");
        request.EnvironmentVariables["DOTNET_ENVIRONMENT"] = "Development";

        await using var session = await sut.StartSessionAsync(request);

        Assert.AreEqual(1, processFactory.Requests.Count);
        var recordedRequest = processFactory.Requests[0].Request;
        var recordedShell = processFactory.Requests[0].Shell;
        Assert.AreEqual("/workspace/aa98", recordedRequest.WorkingDirectory);
        Assert.AreEqual("/workspace", recordedRequest.WorkspaceRootPath);
        Assert.AreEqual("Development", recordedRequest.EnvironmentVariables["DOTNET_ENVIRONMENT"]);
        Assert.AreEqual("bash", recordedShell.DisplayName);
        Assert.AreEqual("/bin/bash", recordedShell.ExecutablePath);
        CollectionAssert.AreEqual(new[] { "-l" }, recordedShell.Arguments.ToArray());
        Assert.IsFalse(recordedShell.IsFallback);
    }

    [TestMethod]
    public async Task HostedTerminalSession_ShouldForwardInputAndStopProcess()
    {
        var hostedProcess = new TestHostedTerminalProcess();
        var sut = new HostedTerminalSession(new TerminalShellDescriptor
        {
            DisplayName = "cmd",
            ExecutablePath = "cmd.exe",
        }, hostedProcess);

        await sut.WriteInputAsync("pwd");
        await sut.StopAsync();

        Assert.AreEqual(1, hostedProcess.WrittenLines.Count);
        Assert.AreEqual("pwd", hostedProcess.WrittenLines[0]);
        Assert.IsTrue(hostedProcess.StopRequested);
        Assert.AreEqual(TerminalSessionState.Stopped, sut.State);
    }

    [TestMethod]
    public async Task HostedTerminalSession_ShouldRaiseOutputAndExitEvents()
    {
        var hostedProcess = new TestHostedTerminalProcess();
        var sut = new HostedTerminalSession(new TerminalShellDescriptor(), hostedProcess);
        string? output = null;
        int? exitCode = null;
        sut.OutputReceived += (_, text) => output = text;
        sut.Exited += (_, code) => exitCode = code;

        hostedProcess.EmitStandardOutput("hello");
        hostedProcess.EmitExited(7);
        await Task.CompletedTask;

        Assert.AreEqual("hello", output);
        Assert.AreEqual(7, exitCode);
        Assert.AreEqual(TerminalSessionState.Stopped, sut.State);
    }
}