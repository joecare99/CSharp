using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent.Application;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.Services;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Desktop.Models;
using Ollama.CodingAgent.Desktop.Services;
using Ollama.CodingAgent.Desktop.ViewModels;

namespace Ollama.CodingAgent.Desktop.Tests;

[TestClass]
public sealed class DesktopSessionViewModelTests
{
    [TestMethod]
    public async Task ApproveCommand_DelegatesToTheSharedSessionApprovalService()
    {
        IAgentSessionStore sessionStore = Substitute.For<IAgentSessionStore>();
        AgentApprovalService approvals = new();
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            sessionStore,
            approvals,
            "desktop-session",
            ".");
        DesktopSessionViewModel viewModel = new(
            session,
            approvals,
            new LocalKnowledgeBaseStore(Path.Combine(Environment.CurrentDirectory, ".agent", "desktop-test-wiki.json")),
            new LocalWikiMarkdownImporter(),
            CreateOptions());
        Assert.AreEqual("test-model", viewModel.EditableModel);
        AgentApprovalRequest request = new()
        {
            Id = "approval-1",
            Operation = "stage",
            Preview = "git add Program.cs",
            CreatedAt = DateTimeOffset.UtcNow,
        };

        Task<bool> decision = approvals.RequestApprovalAsync(request);
        viewModel.RefreshApprovalsCommand.Execute(null);

        Assert.AreEqual(1, viewModel.PendingApprovals.Count);
        viewModel.ApproveCommand.Execute(viewModel.PendingApprovals[0]);

        Assert.IsTrue(await decision);
        Assert.AreEqual(0, viewModel.PendingApprovals.Count);
        Assert.AreEqual("Operation approved.", session.Status);
    }

    [TestMethod]
    public void Parse_UsesDesktopConfigurationForSessionAndCodeWikiVault()
    {
        DesktopOptions options = DesktopOptions.Parse(
        [
            "--endpoint", "http://localhost:11434",
            "--model", "test-model",
            "--workspace", ".",
            "--session", "desktop-session",
            "--code-wiki-vault", ".",
        ]);

        Assert.AreEqual("http://localhost:11434/", options.Endpoint);
        Assert.AreEqual("test-model", options.Model);
        Assert.AreEqual("desktop-session", options.SessionId);
        Assert.AreEqual(Environment.CurrentDirectory, options.WorkspacePath);
        Assert.AreEqual(Environment.CurrentDirectory, options.CodeWikiVaultPath);
    }

    [TestMethod]
    public void DiagnosticsChannel_ProjectsThinkingAndToolActivityIntoSession()
    {
        IAgentSessionStore sessionStore = Substitute.For<IAgentSessionStore>();
        AgentApprovalService approvals = new();
        AgentDiagnosticsChannel channel = new();
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            sessionStore,
            approvals,
            "desktop-session",
            ".",
            channel);

        channel.Record(new Ollama.CodingAgent.Models.AgentDiagnosticEvent
        {
            CorrelationId = "run-1",
            EventName = "tool.call.requested",
            Detail = "The model requested a registered tool.",
        });

        Assert.AreEqual(1, session.Activities.Count);
        Assert.AreEqual("tool.call.requested", session.Activities[0].EventName);
    }

    private static DesktopOptions CreateOptions()
        => new()
        {
            Endpoint = "http://localhost:11434/",
            Model = "test-model",
            WorkspacePath = Environment.CurrentDirectory,
            SessionId = "desktop-session",
            CodeWikiVaultPath = Environment.CurrentDirectory,
        };
}
