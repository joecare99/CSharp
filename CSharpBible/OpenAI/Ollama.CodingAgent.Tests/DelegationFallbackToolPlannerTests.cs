using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class DelegationFallbackToolPlannerTests
{
    [TestMethod]
    public void CreateFallbackToolCall_SelectsTestToolForTestPrompts()
    {
        var call = DelegationFallbackToolPlanner.CreateFallbackToolCall("Please run tests for the coding agent.");

        Assert.AreEqual("run_dotnet_test", call.ToolName);
    }

    [TestMethod]
    public void CreateFallbackToolCall_SelectsBuildToolForBuildPrompts()
    {
        var call = DelegationFallbackToolPlanner.CreateFallbackToolCall("Build the current solution.");

        Assert.AreEqual("run_dotnet_build", call.ToolName);
    }

    [TestMethod]
    public void CreateFallbackToolCall_DefaultsToWorkspaceListing()
    {
        var call = DelegationFallbackToolPlanner.CreateFallbackToolCall("Inspect and summarize the workspace.");

        Assert.AreEqual("list_workspace_files", call.ToolName);
    }
}
