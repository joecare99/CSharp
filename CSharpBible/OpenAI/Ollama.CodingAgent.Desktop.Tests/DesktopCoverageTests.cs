using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent;
using Ollama.CodingAgent.Application;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.Services;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Desktop.Host;
using Ollama.CodingAgent.Desktop.Models;
using Ollama.CodingAgent.Desktop.Services;
using Ollama.CodingAgent.Desktop.ViewModels;
using Ollama.CodingAgent.Desktop;

namespace Ollama.CodingAgent.Desktop.Tests;

[TestClass]
public sealed class DesktopCoverageTests
{
    [TestMethod]
    public void DesktopOptions_ParseAllAliasesAndRejectsUnsafeArguments()
    {
        DesktopOptions options = DesktopOptions.Parse(
        [
            "--endpoint", "https://example.test",
            "--model", "model",
            "--workspace-root", ".",
            "--session", "session",
            "--code-wiki-vault", ".",
        ]);

        Assert.AreEqual("https://example.test/", options.Endpoint);
        Assert.AreEqual("model", options.Model);
        Assert.AreEqual("session", options.SessionId);
        Assert.IsTrue(options.ToRuntimeOptions().WorkspaceRoot.Length > 0);
        Assert.ThrowsExactly<ArgumentException>(() => DesktopOptions.Parse(["--endpoint", "ftp://example.test"]));
        Assert.ThrowsExactly<ArgumentException>(() => DesktopOptions.Parse(["--endpoint", "not-a-uri"]));
        Assert.ThrowsExactly<ArgumentException>(() => DesktopOptions.Parse(["--session", ".."]));
        Assert.ThrowsExactly<ArgumentException>(() => DesktopOptions.Parse(["--session", "nested/session"]));
        Assert.ThrowsExactly<ArgumentException>(() => DesktopOptions.Parse(["--model"]));
        Assert.ThrowsExactly<ArgumentException>(() => DesktopOptions.Parse(["--unknown"]));
    }

    [TestMethod]
    public async Task DesktopSession_ImportsSearchesAndHandlesApprovalOutcomes()
    {
        using TestWorkspace workspace = new();
        string vault = workspace.PathFor("vault");
        Directory.CreateDirectory(vault);
        await File.WriteAllTextAsync(Path.Combine(vault, "page.md"), "# Page\n\nContent");
        LocalKnowledgeBaseStore knowledgeStore = new(workspace.PathFor("knowledge.json"));
        AgentApprovalService approvals = new();
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            Substitute.For<IAgentSessionStore>(),
            approvals,
            "desktop",
            workspace.RootPath);
        DesktopSessionViewModel viewModel = new(
            session,
            approvals,
            knowledgeStore,
            new LocalWikiMarkdownImporter(),
            new DesktopOptions
            {
                Endpoint = "http://localhost:11434/",
                Model = "model",
                WorkspacePath = workspace.RootPath,
                SessionId = "desktop",
                CodeWikiVaultPath = vault,
            });

        await viewModel.ImportCodeWikiVaultCommand.ExecuteAsync(null);
        StringAssert.Contains(viewModel.WikiStatus, "Imported 1 Markdown pages");
        viewModel.WikiQuery = "page";
        await viewModel.SearchWikiCommand.ExecuteAsync(null);
        Assert.AreEqual(1, viewModel.WikiSearchResults.Count);
        StringAssert.Contains(viewModel.WikiStatus, "Found 1");

        viewModel.WikiQuery = string.Empty;
        await viewModel.SearchWikiCommand.ExecuteAsync(null);
        StringAssert.Contains(viewModel.WikiStatus, "Wiki search failed");

        Task<bool> pending = approvals.RequestApprovalAsync(new AgentApprovalRequest
        {
            Id = "request",
            Operation = "operation",
            Preview = "preview",
            CreatedAt = DateTimeOffset.UtcNow,
        });
        viewModel.RefreshApprovalsCommand.Execute(null);
        viewModel.RejectCommand.Execute(viewModel.PendingApprovals[0]);
        Assert.IsFalse(await pending);
        Assert.AreEqual("Operation rejected.", session.Status);

        viewModel.ApproveCommand.Execute(null);
        viewModel.RejectCommand.Execute(null);
        viewModel.ApproveCommand.Execute(new AgentApprovalRequest
        {
            Id = "missing",
            Operation = "operation",
            Preview = "preview",
            CreatedAt = DateTimeOffset.UtcNow,
        });
        Assert.AreEqual("The selected approval is no longer pending.", viewModel.WikiStatus);
    }

    [TestMethod]
    public async Task DesktopSession_ProjectsStateAndReportsServiceFailures()
    {
        using TestWorkspace workspace = new();
        AgentApprovalService approvals = new();
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            Substitute.For<IAgentSessionStore>(),
            approvals,
            "desktop",
            workspace.RootPath);
        LocalKnowledgeBaseStore store = new(workspace.PathFor("knowledge.json"));
        DesktopSessionViewModel viewModel = new(
            session,
            approvals,
            store,
            new LocalWikiMarkdownImporter(),
            new DesktopOptions
            {
                Endpoint = "http://localhost:11434/",
                Model = "model",
                WorkspacePath = workspace.RootPath,
                SessionId = "desktop",
                CodeWikiVaultPath = workspace.PathFor("missing-vault"),
            });

        Assert.AreEqual("Ready.", viewModel.Status);
        Assert.IsNull(viewModel.ErrorMessage);
        Assert.IsFalse(viewModel.HasErrorMessage);
        Assert.IsFalse(viewModel.SearchWikiCommand.CanExecute(null));
        Assert.AreEqual("Planning activity is represented by the visible Thinking entries in the transcript.", viewModel.PlanActivityMessage);
        Assert.AreEqual("Tool calls are sent through the native Ollama tool contract when a tool-enabled loop is active.", viewModel.ToolActivityMessage);
        Assert.AreEqual("Git readiness and repository status are not exposed to desktop clients yet.", viewModel.GitStatusMessage);

        viewModel.WikiQuery = "query";
        Assert.IsTrue(viewModel.SearchWikiCommand.CanExecute(null));
        viewModel.WikiQuery = " ";
        Assert.IsFalse(viewModel.SearchWikiCommand.CanExecute(null));
        viewModel.WikiQuery = "query";
        await viewModel.SearchWikiCommand.ExecuteAsync(null);
        StringAssert.Contains(viewModel.WikiStatus, "Found 0");

        viewModel.CodeWikiVaultPath = workspace.PathFor("missing-vault");
        await viewModel.ImportCodeWikiVaultCommand.ExecuteAsync(null);
        StringAssert.Contains(viewModel.WikiStatus, "CodeWikiVault import failed");

        await File.WriteAllTextAsync(workspace.PathFor("knowledge.json"), "{ malformed json");
        await viewModel.SearchWikiCommand.ExecuteAsync(null);
        StringAssert.Contains(viewModel.WikiStatus, "Wiki search failed");

        session.ErrorMessage = "runtime failure";
        session.Status = "Failed.";
        Assert.IsTrue(viewModel.HasErrorMessage);
        Assert.AreEqual("Failed.", viewModel.Status);
        session.IsRunning = true;
        session.IsRunning = false;
    }

    [TestMethod]
    public void DesktopSession_RejectsMissingDependencies()
    {
        using TestWorkspace workspace = new();
        AgentApprovalService approvals = new();
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            Substitute.For<IAgentSessionStore>(),
            approvals,
            "desktop",
            workspace.RootPath);
        LocalKnowledgeBaseStore store = new(workspace.PathFor("knowledge.json"));
        LocalWikiMarkdownImporter importer = new();
        DesktopOptions options = new()
        {
            Endpoint = "http://localhost:11434/",
            Model = "model",
            WorkspacePath = workspace.RootPath,
            SessionId = "desktop",
            CodeWikiVaultPath = workspace.RootPath,
        };

        Assert.ThrowsExactly<ArgumentNullException>(() => new DesktopSessionViewModel(null!, approvals, store, importer, options));
        Assert.ThrowsExactly<ArgumentNullException>(() => new DesktopSessionViewModel(session, null!, store, importer, options));
        Assert.ThrowsExactly<ArgumentNullException>(() => new DesktopSessionViewModel(session, approvals, null!, importer, options));
        Assert.ThrowsExactly<ArgumentNullException>(() => new DesktopSessionViewModel(session, approvals, store, null!, options));
        Assert.ThrowsExactly<ArgumentNullException>(() => new DesktopSessionViewModel(session, approvals, store, importer, null!));
    }
}
