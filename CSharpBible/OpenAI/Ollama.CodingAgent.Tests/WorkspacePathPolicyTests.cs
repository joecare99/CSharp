using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class WorkspacePathPolicyTests
{
    [TestMethod]
    public void ResolveWorkspacePath_AllowsPathInsideWorkspace()
    {
        using TestWorkspace workspace = new();
        string workspaceRoot = workspace.RootPath;
        WorkspacePathPolicy policy = new(workspaceRoot);

        string resolved = policy.ResolveWorkspacePath("sub\\file.txt");

        Assert.IsTrue(resolved.StartsWith(Path.GetFullPath(workspaceRoot), StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void ResolveWorkspacePath_BlocksPathOutsideWorkspace()
    {
        using TestWorkspace workspace = new();
        string workspaceRoot = workspace.RootPath;
        WorkspacePathPolicy policy = new(workspaceRoot);

        Assert.ThrowsExactly<InvalidOperationException>(() => policy.ResolveWorkspacePath("..\\outside.txt"));
    }

    [TestMethod]
    public void ResolveWorkspacePath_UsesWorkspaceRootForWhitespaceInput()
    {
        using TestWorkspace workspace = new();
        string workspaceRoot = workspace.RootPath;
        WorkspacePathPolicy policy = new(workspaceRoot);

        string resolved = policy.ResolveWorkspacePath("   ");

        Assert.AreEqual(Path.GetFullPath(workspaceRoot), resolved);
    }
}
