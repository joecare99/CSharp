using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class ToolCallParserTests
{
    [TestMethod]
    public void Parse_AcceptsPlainJson()
    {
        var toolCall = ToolCallParser.Parse("{\"toolName\":\"read_workspace_file\",\"input\":\"{\\\"relativePath\\\":\\\"README.md\\\"}\"}");

        Assert.AreEqual("read_workspace_file", toolCall.ToolName);
    }

    [TestMethod]
    public void Parse_AcceptsFencedJson()
    {
        string content = "```json\n{\"toolName\":\"list_workspace_files\",\"input\":\"{}\"}\n```";
        var toolCall = ToolCallParser.Parse(content);

        Assert.AreEqual("list_workspace_files", toolCall.ToolName);
    }

    [TestMethod]
    public void Parse_AcceptsSnakeCaseWithObjectArguments()
    {
        string content = "{\"tool_name\":\"read_workspace_file\",\"arguments\":{\"relativePath\":\"README.md\",\"startLine\":1,\"lineCount\":10}}";
        var toolCall = ToolCallParser.Parse(content);

        Assert.AreEqual("read_workspace_file", toolCall.ToolName);
        StringAssert.Contains(toolCall.Input, "relativePath");
    }

    [TestMethod]
    public void Parse_IgnoresTrailingTextAfterJsonObject()
    {
        string content = "{\"toolName\":\"list_workspace_files\",\"input\":\"{}\"}\nExtra commentary";
        var toolCall = ToolCallParser.Parse(content);

        Assert.AreEqual("list_workspace_files", toolCall.ToolName);
    }

    [TestMethod]
    public void Parse_ThrowsForMissingJson()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() => ToolCallParser.Parse("No JSON here"));
    }
}
