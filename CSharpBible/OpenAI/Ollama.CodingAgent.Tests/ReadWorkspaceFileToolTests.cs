using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class ReadWorkspaceFileToolTests
{
    [TestMethod]
    public async Task ExecuteAsync_ReturnsRequestedLineRange()
    {
        using TestWorkspace workspace = new();
        string root = workspace.RootPath;
        string filePath = Path.Combine(root, "sample.txt");
        await File.WriteAllLinesAsync(filePath, ["a", "b", "c", "d"]);

        ReadWorkspaceFileTool tool = new(new WorkspacePathPolicy(root));
        string input = JsonSerializer.Serialize(new ReadWorkspaceFileToolInput
        {
            RelativePath = "sample.txt",
            StartLine = 2,
            LineCount = 2,
        });

        var result = await tool.ExecuteAsync(input);

        Assert.IsTrue(result.Success);
        StringAssert.Contains(result.Output, "2: b");
        StringAssert.Contains(result.Output, "3: c");
    }
}
