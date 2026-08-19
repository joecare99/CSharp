using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.CodingAgent.Console.Commands;

namespace Ollama.CodingAgent.Console.Tests;

[TestClass]
public sealed class ConsoleCommandParserTests
{
    [TestMethod]
    public void Parse_RecognizesQuotedApprovalId()
    {
        ConsoleCommandParseResult result = ConsoleCommandParser.Parse(":approve \"git-123\"");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Command);
        Assert.AreEqual(ConsoleCommandKind.Approve, result.Command.Kind);
        Assert.AreEqual("git-123", result.Command.Argument);
    }

    [TestMethod]
    public void Parse_RecognizesSlashConfigWithValues()
    {
        ConsoleCommandParseResult result = ConsoleCommandParser.Parse("/config http://localhost:11434 | model | /workspace");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Command);
        Assert.AreEqual(ConsoleCommandKind.Config, result.Command.Kind);
        Assert.AreEqual("http://localhost:11434 | model | /workspace", result.Command.Argument);
    }

    [TestMethod]
    public void Parse_KeepsOrdinaryInputAsPrompt()
    {
        ConsoleCommandParseResult result = ConsoleCommandParser.Parse("  inspect this workspace  ");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Command);
        Assert.AreEqual(ConsoleCommandKind.Prompt, result.Command.Kind);
        Assert.AreEqual("inspect this workspace", result.Command.Argument);
    }

    [TestMethod]
    [DataRow(":approve")]
    [DataRow(":status unexpected")]
    [DataRow(":reject \"unfinished")]
    public void Parse_RejectsMalformedCommands(string input)
    {
        ConsoleCommandParseResult result = ConsoleCommandParser.Parse(input);

        Assert.IsFalse(result.Success);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Error));
    }
}
