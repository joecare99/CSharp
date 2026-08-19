using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.Application.Tests;

[TestClass]
public sealed class AgentSessionViewModelTests
{
    [TestMethod]
    public async Task SubmitCommand_AddsVisibleTurnsAndPersistsSnapshot()
    {
        IAgentSessionService sessionService = Substitute.For<IAgentSessionService>();
        sessionService.RunAsync("Explain the change.", Arg.Any<CancellationToken>()).Returns(Task.FromResult(new AgentRunResult
        {
            FinalResponse = "The change is complete.",
            IterationsUsed = 1,
            RetryAttemptsUsed = 0,
            FinalizedWithMarker = false,
        }));
        IAgentSessionStore sessionStore = Substitute.For<IAgentSessionStore>();
        AgentSessionViewModel viewModel = new(
            sessionService,
            sessionStore,
            new AgentApprovalService(),
            "session-1",
            ".");
        viewModel.Prompt = "Explain the change.";

        await viewModel.SubmitCommand.ExecuteAsync(null);

        Assert.AreEqual(2, viewModel.Conversation.Count);
        Assert.AreEqual(AgentConversationRole.User, viewModel.Conversation[0].Role);
        Assert.AreEqual("The change is complete.", viewModel.Conversation[1].Content);
        Assert.AreEqual("Agent response completed.", viewModel.Status);
        await sessionStore.Received(1).SaveAsync(
            Arg.Is<AgentSessionSnapshot>(snapshot =>
                snapshot.SessionId == "session-1"
                && snapshot.Conversation.Count == 2),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task SubmitCommand_ProjectsLiveThinkingBeforeRunCompletes()
    {
        IAgentSessionService sessionService = Substitute.For<IAgentSessionService>();
        TaskCompletionSource<bool> completionStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<AgentRunResult> runCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        sessionService.RunAsync("Explain the change.", Arg.Any<CancellationToken>())
            .Returns(_ => runCompletion.Task);
        IAgentSessionStore sessionStore = Substitute.For<IAgentSessionStore>();
        AgentSessionViewModel viewModel = new(
            sessionService,
            sessionStore,
            new AgentApprovalService(),
            "session-1",
            ".")
        {
            Prompt = "Explain the change.",
        };

        Task submitTask = viewModel.SubmitCommand.ExecuteAsync(null);
        completionStarted.SetResult(true);
        await completionStarted.Task;

        Assert.AreEqual(1, viewModel.Conversation.Count);
        Assert.AreEqual("Explain the change.", viewModel.Conversation[0].Content);

        runCompletion.SetResult(new AgentRunResult
        {
            FinalResponse = "Done.",
            IterationsUsed = 1,
            RetryAttemptsUsed = 0,
            FinalizedWithMarker = false,
        });
        await submitTask;
    }

    [TestMethod]
    public async Task ReloadCommand_RejectsSnapshotFromDifferentSession()
    {
        IAgentSessionStore sessionStore = Substitute.For<IAgentSessionStore>();
        sessionStore.LoadAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new AgentSessionSnapshot
        {
            SessionId = "other-session",
            WorkspacePath = ".",
        }));
        AgentSessionViewModel viewModel = new(
            Substitute.For<IAgentSessionService>(),
            sessionStore,
            new AgentApprovalService(),
            "session-1",
            ".");

        await Assert.ThrowsExactlyAsync<System.InvalidOperationException>(() => viewModel.ReloadCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task JsonStore_PersistsAndResumesConversation()
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "TestSessions", System.Guid.NewGuid().ToString("N"));
        string filePath = Path.Combine(directory, "session.json");
        try
        {
            JsonAgentSessionStore store = new(filePath);
            IAgentSessionService sessionService = Substitute.For<IAgentSessionService>();
            sessionService.RunAsync("Hello", Arg.Any<CancellationToken>()).Returns(Task.FromResult(new AgentRunResult
            {
                FinalResponse = "Welcome back.",
                IterationsUsed = 1,
                RetryAttemptsUsed = 0,
                FinalizedWithMarker = false,
            }));
            AgentSessionViewModel originalSession = new(
                sessionService,
                store,
                new AgentApprovalService(),
                "session-1",
                directory)
            {
                Prompt = "Hello",
            };
            await originalSession.SubmitCommand.ExecuteAsync(null);

            AgentSessionViewModel resumedSession = new(
                Substitute.For<IAgentSessionService>(),
                store,
                new AgentApprovalService(),
                "session-1",
                directory);
            await resumedSession.ReloadCommand.ExecuteAsync(null);

            Assert.AreEqual("Session reloaded.", resumedSession.Status);
            Assert.AreEqual(2, resumedSession.Conversation.Count);
            Assert.AreEqual("Hello", resumedSession.Conversation[0].Content);
            Assert.AreEqual("Welcome back.", resumedSession.Conversation[1].Content);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task ReloadCommand_RejectsSnapshotFromDifferentWorkspace()
    {
        IAgentSessionStore sessionStore = Substitute.For<IAgentSessionStore>();
        sessionStore.LoadAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new AgentSessionSnapshot
        {
            SessionId = "session-1",
            WorkspacePath = Path.Combine(AppContext.BaseDirectory, "other-workspace"),
        }));
        AgentSessionViewModel viewModel = new(
            Substitute.For<IAgentSessionService>(),
            sessionStore,
            new AgentApprovalService(),
            "session-1",
            Path.Combine(AppContext.BaseDirectory, "workspace"));

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => viewModel.ReloadCommand.ExecuteAsync(null));
    }
}
