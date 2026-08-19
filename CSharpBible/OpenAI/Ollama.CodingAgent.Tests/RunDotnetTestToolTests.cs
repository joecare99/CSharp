using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class RunDotnetTestToolTests
{
    [TestMethod]
    public void Validate_RejectsUnsupportedFileExtension()
    {
        RunDotnetTestTool tool = new(new WorkspacePathPolicy("C:\\"));

        var result = tool.Validate("{\"relativePath\":\"Windows\\\\not-allowed.txt\"}");

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Count > 0);
    }
}
