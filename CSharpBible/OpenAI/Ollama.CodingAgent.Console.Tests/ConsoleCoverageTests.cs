using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BaseLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.CodingAgent.Console.Commands;
using NSubstitute;
using Ollama.CodingAgent.Application;

namespace Ollama.CodingAgent.Console.Tests;

[TestClass]
public sealed class ConsoleCoverageTests
{
    [TestMethod]
    public void ParserProjectionAndOptions_CoverSupportedTerminalInput()
    {
        foreach (string command in new[] { ":help", ":status", ":transcript", ":reload", ":clear", ":cancel", ":approvals", ":exit", ":quit" })
        {
            Assert.IsTrue(ConsoleCommandParser.Parse(command).Success);
        }

        Assert.AreEqual("hello world", ConsoleCommandParser.Parse(":prompt \"hello world\"").Command!.Argument);
        Assert.AreEqual("a\\b", ConsoleCommandParser.Parse(":approve \"a\\\\b\"").Command!.Argument);
        Assert.IsFalse(ConsoleCommandParser.Parse(":").Success);
        Assert.IsFalse(ConsoleCommandParser.Parse(@":approve value\q").Success);
        Assert.IsFalse(ConsoleCommandParser.Parse(@":approve value\").Success);
        Assert.IsFalse(ConsoleCommandParser.Parse(":prompt").Success);
        Assert.IsTrue(ConsoleCommandParser.Parse(null).Success);
        Assert.IsTrue(ConsoleCommandParser.Parse("   ").Success);
        Assert.IsFalse(ConsoleCommandParser.Parse(":approve").Success);
        Assert.IsFalse(ConsoleCommandParser.Parse(":reject").Success);
        Assert.IsFalse(ConsoleCommandParser.Parse(":approve \"\"").Success);
        Assert.IsFalse(ConsoleCommandParser.Parse(":reject \"\"").Success);
        Assert.IsTrue(ConsoleCommandParser.Parse(":prompt \"\"").Success);
        foreach (string malformedKnownCommand in new[]
        {
            ":help extra",
            ":status extra",
            ":transcript extra",
            ":reload extra",
            ":clear extra",
            ":cancel extra",
            ":approvals extra",
            ":exit extra",
            ":quit extra",
            ":approve one two",
            ":reject one two",
        })
        {
            Assert.IsFalse(ConsoleCommandParser.Parse(malformedKnownCommand).Success);
        }

        Assert.IsTrue(ConsoleCommandParser.Parse(":prompt one \"two three\"").Success);
        Assert.IsTrue(ConsoleCommandParser.Parse(":approve \"one\\\"two\"").Success);

        AgentApprovalRequest approval = new()
        {
            Id = "approval",
            Operation = "commit",
            Preview = "commit changes",
            CreatedAt = DateTimeOffset.UnixEpoch,
        };
        StringAssert.Contains(AgentSessionProjection.FormatApprovals([approval]), "commit changes");
        Assert.AreEqual("No pending approvals.", AgentSessionProjection.FormatApprovals([]));
        string transcript = AgentSessionProjection.FormatTranscript(
        [
            new AgentConversationTurn { Role = AgentConversationRole.System, Content = "system", CreatedAt = DateTimeOffset.UnixEpoch },
            new AgentConversationTurn { Role = (AgentConversationRole)99, Content = "unknown", CreatedAt = DateTimeOffset.UnixEpoch },
        ]);
        StringAssert.Contains(transcript, "system:");
        StringAssert.Contains(transcript, "unknown:");

        Assert.IsTrue(ConsoleAgentCliOptions.Parse(["--help"]).ShowHelp);
        Assert.IsTrue(ConsoleAgentCliOptions.Parse(["--workspace-root", "."]).ToRuntimeOptions().WorkspaceRoot.Length > 0);
        Assert.ThrowsExactly<ArgumentException>(() => ConsoleAgentCliOptions.Parse(["--endpoint", "ftp://example.test"]));
        Assert.ThrowsExactly<ArgumentException>(() => ConsoleAgentCliOptions.Parse(["--endpoint", "not a URI"]));
        Assert.ThrowsExactly<ArgumentException>(() => ConsoleAgentCliOptions.Parse(["--model"]));
        Assert.ThrowsExactly<ArgumentException>(() => ConsoleAgentCliOptions.Parse(["--unknown"]));
    }

    [TestMethod]
    public async Task Repl_RunsAllSafeCommandsAndProjectsResponses()
    {
        IAgentSessionService sessionService = Substitute.For<IAgentSessionService>();
        sessionService.RunAsync("prompt", Arg.Any<CancellationToken>()).Returns(Task.FromResult(new Ollama.CodingAgent.Models.AgentRunResult
        {
            FinalResponse = "answer",
            IterationsUsed = 1,
            RetryAttemptsUsed = 0,
            FinalizedWithMarker = false,
        }));
        IAgentSessionStore store = Substitute.For<IAgentSessionStore>();
        store.LoadAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new AgentSessionSnapshot
        {
            SessionId = "console",
            WorkspacePath = ".",
        }));
        AgentApprovalService approvals = new();
        AgentSessionViewModel session = new(sessionService, store, approvals, "console", ".");
        IConsole console = Substitute.For<IConsole>();
        console.ReadLine().Returns(
            ":help",
            ":status",
            ":transcript",
            ":approvals",
            ":approve missing",
            ":reject missing",
            ":cancel",
            ":reload",
            ":clear",
            "prompt",
            ":exit");

        await new ConsoleRepl(session, approvals, console).RunAsync();

        console.Received().WriteLine(Arg.Is<string>(value => value.Contains("No agent request is active.", StringComparison.Ordinal)));
        console.Received().WriteLine(Arg.Is<string>(value => value.Contains("assistant> answer", StringComparison.Ordinal)));
        await sessionService.Received(1).RunAsync("prompt", Arg.Any<CancellationToken>());
    }
}
