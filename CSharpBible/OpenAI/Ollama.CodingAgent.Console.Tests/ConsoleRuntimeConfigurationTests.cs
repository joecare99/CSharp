using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.CodingAgent.Console.Configuration;

namespace Ollama.CodingAgent.Console.Tests;

[TestClass]
public sealed class ConsoleRuntimeConfigurationTests
{
    [TestMethod]
    public void Set_NormalizesEndpointModelAndWorkspace()
    {
        ConsoleRuntimeConfiguration configuration = new(".", "", "");

        configuration.Set("http://localhost:11434", " model ", ".");

        Assert.AreEqual("http://localhost:11434/", configuration.Endpoint);
        Assert.AreEqual("model", configuration.Model);
        Assert.AreEqual(Environment.CurrentDirectory, configuration.WorkspacePath);
    }

    [TestMethod]
    public void Set_RejectsNonHttpEndpoint()
    {
        ConsoleRuntimeConfiguration configuration = new(".", "", "");

        Assert.ThrowsExactly<ArgumentException>(() => configuration.Set("file:///tmp", "model", "."));
    }
}