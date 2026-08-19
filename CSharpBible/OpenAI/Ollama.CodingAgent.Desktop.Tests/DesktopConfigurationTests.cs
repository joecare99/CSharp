using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.CodingAgent.Desktop.Models;
using Ollama.CodingAgent.Desktop.Services;

namespace Ollama.CodingAgent.Desktop.Tests;

[TestClass]
public sealed class DesktopConfigurationTests
{
    [TestMethod]
    public async Task Store_RememberAsync_PersistsNewestEndpointFirstAndLimitsEntries()
    {
        string directory = Path.Combine(Path.GetTempPath(), "ollama-coding-agent-tests", Guid.NewGuid().ToString("N"));
        string filePath = Path.Combine(directory, "configurations.json");
        try
        {
            DesktopConfigurationStore store = new(filePath);
            for (int index = 0; index < 12; index++)
            {
                await store.RememberAsync(new DesktopConfiguration
                {
                    Endpoint = $"http://localhost:{11434 + index}/",
                    Model = $"model-{index}",
                    WorkspacePath = directory,
                });
            }

            var configurations = await store.LoadAsync();
            Assert.AreEqual(10, configurations.Count);
            Assert.AreEqual("http://localhost:11445/", configurations[0].Endpoint);
            Assert.AreEqual("model-2", configurations[^1].Model);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    [TestMethod]
    public void Configuration_Normalize_RejectsNonHttpEndpoint()
    {
        DesktopConfiguration configuration = new()
        {
            Endpoint = "file:///tmp/ollama",
            Model = "test-model",
            WorkspacePath = Environment.CurrentDirectory,
        };

        Assert.ThrowsExactly<ArgumentException>(() => configuration.Normalize());
    }
}
