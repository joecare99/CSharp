using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BaseLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Services;

namespace Ollama.CodingAgent.Console.Tests;

[TestClass]
public sealed class ConsoleReplCoverageTests
{
    [TestMethod]
    public async Task ExecuteAsync_CoversEveryCommandOutcome()
    {
        IAgentSessionService sessionService = Substitute.For<IAgentSessionService>();
        sessionService.RunAsync("prompt", Arg.Any<CancellationToken>()).Returns(Task.FromResult(new AgentRunResult
        {
            FinalResponse = "answer",
            IterationsUsed = 1,
            RetryAttemptsUsed = 0,
            FinalizedWithMarker = false,
        }));
        sessionService.RunAsync("failure", Arg.Any<CancellationToken>())
            .Returns(Task.FromException<AgentRunResult>(new InvalidOperationException("failure")));
        using CancellationTokenSource cancelled = new();
        cancelled.Cancel();
        sessionService.RunAsync("cancelled", Arg.Any<CancellationToken>())
            .Returns(Task.FromCanceled<AgentRunResult>(cancelled.Token));
        IAgentSessionStore store = Substitute.For<IAgentSessionStore>();
        store.LoadAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new AgentSessionSnapshot
        {
            SessionId = "session",
            WorkspacePath = ".",
        }));
        AgentApprovalService approvals = new();
        AgentSessionViewModel session = new(sessionService, store, approvals, "session", ".");
        IConsole console = Substitute.For<IConsole>();
        ConsoleRepl repl = new(session, approvals, console);

        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Empty));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Prompt, " "));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Prompt, "prompt"));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Prompt, "failure"));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Prompt, "cancelled"));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Help));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Status));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Transcript));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Reload));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Clear));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Cancel));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Approvals));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Approve, "missing"));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Reject, "missing"));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Approve));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Reject, " "));
        Assert.IsTrue(await ExecuteAsync(repl, ConsoleCommandKind.Exit));
        Assert.IsFalse(await ExecuteAsync(repl, (ConsoleCommandKind)999));

        Task<bool> approved = approvals.RequestApprovalAsync(CreateRequest("approved"));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Approve, "approved"));
        Assert.IsTrue(await approved);
        Task<bool> rejected = approvals.RequestApprovalAsync(CreateRequest("rejected"));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Reject, "rejected"));
        Assert.IsFalse(await rejected);

        store.LoadAsync(Arg.Any<CancellationToken>()).Returns(Task.FromException<AgentSessionSnapshot>(new IOException("store failed")));
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Reload));
        console.Received().WriteLine(Arg.Is<string>(value => value.Contains("Operation failed: store failed", StringComparison.Ordinal)));
        console.Received().WriteLine(Arg.Is<string>(value => value.Contains("Unsupported command", StringComparison.Ordinal)));

        session.IsRunning = true;
        Assert.IsFalse(await ExecuteAsync(repl, ConsoleCommandKind.Cancel));
        console.Received().WriteLine("Cancellation requested.");
        InvokeCancelKeyPress(repl, session, console);
    }

    [TestMethod]
    public async Task RunAsync_ReportsMalformedInputBeforeRedirectedInputCloses()
    {
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            Substitute.For<IAgentSessionStore>(),
            new AgentApprovalService(),
            "session",
            ".");
        IConsole console = Substitute.For<IConsole>();
        console.ReadLine().Returns(":unknown", string.Empty);

        await new ConsoleRepl(session, new AgentApprovalService(), console, null, null, () => true).RunAsync();

        console.Received().WriteLine(Arg.Is<string>(value => value.Contains("Input error:", StringComparison.Ordinal)));
    }

    [TestMethod]
    public async Task Program_ExitsWhenRedirectedInputIsEmpty()
    {
        TextReader originalInput = System.Console.In;
        try
        {
            System.Console.SetIn(new StringReader(string.Empty));
            MethodInfo main = typeof(ConsoleAgentCliOptions).Assembly
                .GetType("Ollama.CodingAgent.Console.Program", throwOnError: true)!
                .GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static)!;
            Assert.AreEqual(0, await (Task<int>)main.Invoke(null, [new[] { "--workspace", ".", "--session", "program" }])!);
            Assert.AreEqual(0, await (Task<int>)main.Invoke(null, [new[] { "--help" }])!);
            Assert.AreEqual(1, await (Task<int>)main.Invoke(null, [new[] { "--unknown" }])!);
        }
        finally
        {
            System.Console.SetIn(originalInput);
        }
    }

    private static Task<bool> ExecuteAsync(ConsoleRepl repl, ConsoleCommandKind kind, string? argument = null)
    {
        MethodInfo execute = typeof(ConsoleRepl).GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (Task<bool>)execute.Invoke(repl, [new ConsoleCommand { Kind = kind, Argument = argument }])!;
    }

    private static void InvokeCancelKeyPress(ConsoleRepl repl, AgentSessionViewModel session, IConsole console)
    {
        MethodInfo cancel = typeof(ConsoleRepl).GetMethod("OnCancelKeyPress", BindingFlags.NonPublic | BindingFlags.Instance)!;
        ConsoleCancelEventArgs eventArgs = CreateCancelEventArgs();
        cancel.Invoke(repl, [null, eventArgs]);
        Assert.IsTrue(eventArgs.Cancel);
        console.Received().WriteLine("Cancellation requested.");
        session.IsRunning = false;
        cancel.Invoke(repl, [null, CreateCancelEventArgs()]);
    }

    [TestMethod]
    public void Constructor_RejectsNullRequiredArgumentsAndAcceptsCustomParser()
    {
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            Substitute.For<IAgentSessionStore>(),
            new AgentApprovalService(),
            "session",
            ".");
        IAgentApprovalService approvals = new AgentApprovalService();
        IConsole console = Substitute.For<IConsole>();
        ConstructorInfo constructor = typeof(ConsoleRepl).GetConstructor(
        [
            typeof(AgentSessionViewModel),
            typeof(IAgentApprovalService),
            typeof(IConsole),
            typeof(Func<string, ConsoleCommandParseResult>),
        ])!;

        Assert.ThrowsExactly<TargetInvocationException>(() => constructor.Invoke([null, approvals, console, null]));
        Assert.ThrowsExactly<TargetInvocationException>(() => constructor.Invoke([session, null, console, null]));
        Assert.ThrowsExactly<TargetInvocationException>(() => constructor.Invoke([session, approvals, null, null]));
        Assert.IsNotNull(new ConsoleRepl(session, approvals, console, ConsoleCommandParser.Parse));
    }

    private static ConsoleCancelEventArgs CreateCancelEventArgs()
        => (ConsoleCancelEventArgs)Activator.CreateInstance(
            typeof(ConsoleCancelEventArgs),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: [ConsoleSpecialKey.ControlC],
            culture: null)!;

    private static AgentApprovalRequest CreateRequest(string id)
        => new()
        {
            Id = id,
            Operation = "operation",
            Preview = "preview",
            CreatedAt = DateTimeOffset.UtcNow,
        };
}
