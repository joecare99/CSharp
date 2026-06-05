using System;
using System.IO;
using System.Threading.Tasks;
using Avln_TerminalHost.Services;
using Avln_TerminalHost.ViewModels;
using Avalonia.Headless.MSTest;
using Avalonia.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Avln_TerminalHostTests;

[TestClass]
public class MainWindowViewModelTests
{
    [AvaloniaTestMethod]
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

    [AvaloniaTestMethod]
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

    [AvaloniaTestMethod]
    public async Task ProcessOutput_ShouldBeWrittenToConsole()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var sut = new MainWindowViewModel(processRunner);
        await sut.StartCommand.ExecuteAsync(null);

        hostedProcess.EmitStandardOutput("hello");
        hostedProcess.EmitStandardError("boom");
        await Dispatcher.UIThread.InvokeAsync(() => { });

        sut.Console.Content.Should().Contain("hello");
        sut.Console.Content.Should().Contain("boom");
        sut.Console.Content.Should().Contain("\\c4Fboom");
    }

    [AvaloniaTestMethod]
    public async Task PartialProcessOutput_ShouldBeWrittenWithoutWaitingForLineBreak()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var sut = new MainWindowViewModel(processRunner);
        await sut.StartCommand.ExecuteAsync(null);

        hostedProcess.EmitStandardOutputPartial("C:\\> ");
        await Dispatcher.UIThread.InvokeAsync(() => { });

        sut.Console.Content.Should().Contain("C:\\\\>");
    }

    [TestMethod]
    public async Task OutputChunkReader_ShouldFlushPartialOutputAfterInactivity()
    {
        using var reader = new StringReader("prompt> ");
        var partialOutput = string.Empty;
        var sut = new OutputChunkReader(
            reader,
            TimeSpan.FromMilliseconds(20),
            _ => Assert.Fail("No complete line expected."),
            text => partialOutput += text);

        await sut.RunAsync(default);

        partialOutput.Should().Be("prompt> ");
    }

    [TestMethod]
    public async Task OutputChunkReader_ShouldEmitCompleteLineWithoutPartialFlush()
    {
        using var reader = new StringReader("hello\n");
        var lineOutput = string.Empty;
        var partialOutput = string.Empty;
        var sut = new OutputChunkReader(
            reader,
            TimeSpan.FromMilliseconds(20),
            text => lineOutput += text,
            text => partialOutput += text);

        await sut.RunAsync(default);

        lineOutput.Should().Be("hello");
        partialOutput.Should().BeEmpty();
    }

    [TestMethod]
    public async Task OutputChunkReader_ShouldEmitLineImmediatelyAndKeepTrailingPromptPartial()
    {
        using var reader = new StringReader("hello\nprompt> ");
        var lineOutput = string.Empty;
        var partialOutput = string.Empty;
        var sut = new OutputChunkReader(
            reader,
            TimeSpan.FromMilliseconds(20),
            text => lineOutput += text,
            text => partialOutput += text);

        await sut.RunAsync(default);

        lineOutput.Should().Be("hello");
        partialOutput.Should().Be("prompt> ");
    }

    [AvaloniaTestMethod]
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

    [AvaloniaTestMethod]
    public async Task ConsoleTextInput_ShouldEchoAndTrackCurrentInteractiveInput()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var sut = new MainWindowViewModel(processRunner);
        await sut.StartCommand.ExecuteAsync(null);

        await sut.HandleConsoleTextInputAsync("dir");

        sut.InputText.Should().Be("dir");
        sut.Console.Content.Should().Contain("dir");
    }

    [AvaloniaTestMethod]
    public async Task ConsoleEnter_ShouldForwardBufferedInteractiveInputToProcess()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var sut = new MainWindowViewModel(processRunner);
        await sut.StartCommand.ExecuteAsync(null);
        await sut.HandleConsoleTextInputAsync("dir");

        await sut.HandleConsoleSpecialKeyAsync(ConsoleKey.Enter);

        hostedProcess.WrittenLines.Should().ContainSingle().Which.Should().Be("dir");
        sut.InputText.Should().BeEmpty();
    }

    [AvaloniaTestMethod]
    public async Task ConsoleBackspace_ShouldUpdateBufferedInteractiveInput()
    {
        var hostedProcess = new TestHostedProcess();
        var processRunner = Substitute.For<IProcessRunner>();
        processRunner.StartAsync(default).ReturnsForAnyArgs(Task.FromResult<IHostedProcess>(hostedProcess));
        var sut = new MainWindowViewModel(processRunner);
        await sut.StartCommand.ExecuteAsync(null);
        await sut.HandleConsoleTextInputAsync("dir");

        await sut.HandleConsoleSpecialKeyAsync(ConsoleKey.Backspace);

        sut.InputText.Should().Be("di");
    }

    [AvaloniaTestMethod]
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
