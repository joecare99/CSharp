using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.CodingAgent.Console.Configuration;

namespace Ollama.CodingAgent.Console.Tests;

[TestClass]
public sealed class ConsoleAgentCliOptionsTests
{
    [TestMethod]
    public void Parse_ReadsRequiredTerminalConfiguration()
    {
        ConsoleAgentCliOptions options = ConsoleAgentCliOptions.Parse(
        [
            "--endpoint", "http://localhost:11434",
            "--model", "qwen:test",
            "--workspace", ".",
            "--session", "review-01",
        ]);

        Assert.AreEqual("http://localhost:11434/", options.Endpoint);
        Assert.AreEqual("qwen:test", options.Model);
        Assert.AreEqual("review-01", options.SessionId);
        Assert.IsTrue(System.IO.Path.IsPathFullyQualified(options.WorkspacePath));
    }

    [TestMethod]
    [DataRow("../session")]
    [DataRow("..")]
    [DataRow("session/name")]
    public void Parse_RejectsUnsafeSessionId(string sessionId)
    {
        Assert.ThrowsExactly<ArgumentException>(
            () => ConsoleAgentCliOptions.Parse(["--session", sessionId]));
    }
}
