using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class OllamaAgentCliOptionsTests
{
    [TestMethod]
    public void Parse_ReadsVerbosityAndThinkingOptions()
    {
        OllamaAgentCliOptions options = OllamaAgentCliOptions.Parse(
        [
            "--verbosity", "verbose",
            "--show-thinking",
            "--prompt", "Inspect the workspace.",
        ]);

        Assert.AreEqual(AgentVerbosity.Verbose, options.RuntimeSettings.Verbosity);
        Assert.IsTrue(options.RuntimeSettings.ShowThinking);
        Assert.AreEqual("Inspect the workspace.", options.Prompt);
    }

    [TestMethod]
    public void Parse_RejectsUnknownVerbosity()
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(
            () => OllamaAgentCliOptions.Parse(["--verbosity", "trace"]));

        StringAssert.Contains(exception.Message, "quiet, normal, or verbose");
    }
}
