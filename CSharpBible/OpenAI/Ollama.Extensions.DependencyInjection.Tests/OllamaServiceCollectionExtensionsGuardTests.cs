using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.Extensions.DependencyInjection.Tests;

[TestClass]
public sealed class OllamaServiceCollectionExtensionsGuardTests
{
    [TestMethod]
    public void AddOllamaClient_ThrowsForNullServices()
    {
        IServiceCollection? services = null;

        Assert.ThrowsExactly<ArgumentNullException>(() => Ollama.Extensions.DependencyInjection.OllamaServiceCollectionExtensions.AddOllamaClient(services!, new Uri("http://localhost:11434/")));
    }

    [TestMethod]
    public void AddOllamaChatClient_ThrowsForEmptyModel()
    {
        ServiceCollection services = [];

        Assert.ThrowsExactly<ArgumentException>(() => services.AddOllamaChatClient(string.Empty));
    }

    [TestMethod]
    public void AddOllamaClient_ThrowsForNullEndpoint()
    {
        ServiceCollection services = [];

        Assert.ThrowsExactly<ArgumentNullException>(() => services.AddOllamaClient(null!));
    }

    [TestMethod]
    public void AddOllamaGenerateClient_ThrowsForEmptyModel()
    {
        ServiceCollection services = [];

        Assert.ThrowsExactly<ArgumentException>(() => services.AddOllamaGenerateClient(string.Empty));
    }

    [TestMethod]
    public void AddOllamaEmbeddingClient_ThrowsForEmptyModel()
    {
        ServiceCollection services = [];

        Assert.ThrowsExactly<ArgumentException>(() => services.AddOllamaEmbeddingClient(string.Empty));
    }
}
