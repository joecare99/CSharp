using System.Threading.Tasks;
using Avln_TerminalHost.Services;
using Avln_TerminalHost.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Avln_TerminalHostTests;

[TestClass]
public class MainWindowViewModelTests
{
    [TestMethod]
    public async Task StartCommand_ShouldStartProcessAndAppendStartupMessage()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var sut = new MainWindowViewModel(processRunner);

        await sut.StartCommand.ExecuteAsync(null);

        sut.IsRunning.Should().BeTrue();
        sut.Console.Content.Should().Contain("Started shell process.");
        await processRunner.Received(1).StartAsync(default);
    }

    [TestMethod]
    public async Task SendInputCommand_ShouldForwardInputAndEchoPrompt()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var sut = new MainWindowViewModel(processRunner);
        await sut.StartCommand.ExecuteAsync(null);
        sut.InputText = "dir";

        await sut.SendInputCommand.ExecuteAsync(null);

        hostedProcess.WrittenLines.Should().ContainSingle().Which.Should().Be("dir");
        sut.InputText.Should().BeEmpty();
        sut.Console.Content.Should().Contain("> dir");
    }

    [TestMethod]
    public async Task ProcessOutput_ShouldBeWrittenToConsole()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var sut = new MainWindowViewModel(processRunner);
        await sut.StartCommand.ExecuteAsync(null);

        hostedProcess.EmitStandardOutput("hello");
        hostedProcess.EmitStandardError("boom");

        sut.Console.Content.Should().Contain("hello");
        sut.Console.Content.Should().Contain("boom");
        sut.Console.Content.Should().Contain("\\c4Fboom");
    }

    [TestMethod]
    public async Task StopCommand_ShouldRequestProcessStop()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var sut = new MainWindowViewModel(processRunner);
        await sut.StartCommand.ExecuteAsync(null);

        await sut.StopCommand.ExecuteAsync(null);

        hostedProcess.StopRequested.Should().BeTrue();
        sut.Console.Content.Should().Contain("Stopping shell process.");
    }

    [TestMethod]
    public async Task ExitedEvent_ShouldResetRunningStateAndDisposeProcess()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var sut = new MainWindowViewModel(processRunner);
        await sut.StartCommand.ExecuteAsync(null);

        hostedProcess.EmitExited(7);
        await Task.Delay(50);

        sut.IsRunning.Should().BeFalse();
        hostedProcess.IsDisposed.Should().BeTrue();
        sut.Console.Content.Should().Contain("Process exited with code 7.");
    }
}
