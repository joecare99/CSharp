using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class RunDotnetBuildToolTests
{
    [TestMethod]
    public Task Validate_RejectsUnsupportedFileExtension()
    {
        RunDotnetBuildTool tool = new(new WorkspacePathPolicy("C:\\"));

        var result = tool.Validate("{\"relativePath\":\"Windows\\\\not-allowed.txt\"}");

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Count > 0);
        return Task.CompletedTask;
    }
}
