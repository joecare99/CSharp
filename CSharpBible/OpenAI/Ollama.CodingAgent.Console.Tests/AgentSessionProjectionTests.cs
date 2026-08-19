using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent.Application;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.Services;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Console.Presentation;

namespace Ollama.CodingAgent.Console.Tests;

[TestClass]
public sealed class AgentSessionProjectionTests
{
    [TestMethod]
    public void FormatTranscript_ShowsOnlyVisibleConversation()
    {
        string transcript = AgentSessionProjection.FormatTranscript(
        [
            new AgentConversationTurn
            {
                Role = AgentConversationRole.User,
                Content = "Inspect the change.",
                CreatedAt = DateTimeOffset.UnixEpoch,
            },
            new AgentConversationTurn
            {
                Role = AgentConversationRole.Assistant,
                Content = "The change is safe.",
                CreatedAt = DateTimeOffset.UnixEpoch,
            },
        ]);

        StringAssert.Contains(transcript, "user:");
        StringAssert.Contains(transcript, "Inspect the change.");
        StringAssert.Contains(transcript, "assistant:");
        Assert.IsFalse(transcript.Contains("thinking", StringComparison.Ordinal));
    }

    [TestMethod]
    public void FormatStatus_ProjectsSharedApplicationState()
    {
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            Substitute.For<IAgentSessionStore>(),
            new AgentApprovalService(),
            "console-session",
            ".");

        string status = AgentSessionProjection.FormatStatus(session);

        StringAssert.Contains(status, "Session: console-session");
        StringAssert.Contains(status, "Status: Ready.");
        StringAssert.Contains(status, "Pending approvals: 0");
    }
}
