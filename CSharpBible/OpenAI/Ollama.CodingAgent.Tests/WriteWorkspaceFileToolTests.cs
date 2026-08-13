using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class WriteWorkspaceFileToolTests
{
    [TestMethod]
    public async Task ExecuteAsync_WritesNewFile()
    {
        string root = Path.Combine(Path.GetTempPath(), "write-tool-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        WriteWorkspaceFileTool tool = new(new WorkspacePathPolicy(root));
        string input = JsonSerializer.Serialize(new WriteWorkspaceFileToolInput
        {
            RelativePath = "notes\\sample.md",
            Content = "hello",
            Overwrite = false,
        });

        var result = await tool.ExecuteAsync(input);

        Assert.IsTrue(result.Success);
        string target = Path.Combine(root, "notes", "sample.md");
        Assert.IsTrue(File.Exists(target));
        Assert.AreEqual("hello", await File.ReadAllTextAsync(target));
    }

    [TestMethod]
    public void Validate_BlocksUnsupportedExtension()
    {
        WriteWorkspaceFileTool tool = new(new WorkspacePathPolicy("C:\\"));
        string input = JsonSerializer.Serialize(new WriteWorkspaceFileToolInput
        {
            RelativePath = "Windows\\temp.exe",
            Content = "x",
            Overwrite = true,
        });

        var validation = tool.Validate(input);

        Assert.IsFalse(validation.IsValid);
        Assert.IsTrue(validation.Errors.Count > 0);
    }
}
