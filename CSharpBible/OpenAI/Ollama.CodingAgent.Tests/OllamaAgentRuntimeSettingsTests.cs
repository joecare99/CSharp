using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class OllamaAgentRuntimeSettingsTests
{
    [TestMethod]
    public void DefaultBaselineValues_AreConfiguredForLocalModelExecution()
    {
        Assert.AreEqual(TimeSpan.FromMinutes(12), OllamaAgentRuntimeSettings.DefaultStepTimeout);
        Assert.AreEqual(3, OllamaAgentRuntimeSettings.DefaultRetryCount);
        Assert.AreEqual(80, OllamaAgentRuntimeSettings.DefaultMaxIterations);
    }

    [TestMethod]
    [DataRow(0d, 3, 80)]
    [DataRow(-1d, 3, 80)]
    [DataRow(12d, -1, 80)]
    [DataRow(12d, 3, 0)]
    [DataRow(12d, 3, -5)]
    public void Constructor_RejectsInvalidValues(double timeoutMinutes, int retries, int maxIterations)
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new OllamaAgentRuntimeSettings(
            TimeSpan.FromMinutes(timeoutMinutes),
            retries,
            maxIterations));
    }
}
