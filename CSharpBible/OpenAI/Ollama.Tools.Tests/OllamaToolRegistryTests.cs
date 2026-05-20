using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.Tests.TestDoubles;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class OllamaToolRegistryTests
{
    [TestMethod]
    public void TryGetTool_ReturnsRegisteredTool()
    {
        OllamaToolRegistry registry = new([
            new TestTool
            {
                Name = "clock",
                Description = "Returns the current time.",
                ResultText = "ok",
            },
        ]);

        bool found = registry.TryGetTool("clock", out Ollama.Tools.Abstractions.IOllamaTool? tool);

        Assert.IsTrue(found);
        Assert.IsNotNull(tool);
        Assert.AreEqual("clock", tool.Name);
    }

    [TestMethod]
    public void GetDescriptors_ContainsSchemaMetadata()
    {
        OllamaToolRegistry registry = new([
            new TestTool
            {
                Name = "clock",
                Description = "Returns the current time.",
                ResultText = "ok",
            },
        ]);

        IReadOnlyList<OllamaToolDescriptor> descriptors = registry.GetDescriptors();

        Assert.AreEqual(1, descriptors.Count);
        Assert.AreEqual("Accepts a plain string input.", descriptors[0].Schema.Summary);
        Assert.AreEqual(1, descriptors[0].Schema.Parameters.Count);
        Assert.AreEqual("input", descriptors[0].Schema.Parameters[0].Name);
    }
}
