using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Client.Services;

namespace Ollama.Extensions.DependencyInjection.Tests;

[TestClass]
public sealed class OllamaServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddOllamaClient_RegistersRootClientAndOptions()
    {
        ServiceCollection services = [];
        Uri endpoint = new("http://localhost:11434/");

        services.AddOllamaClient(endpoint);
        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        OllamaClient client = serviceProvider.GetRequiredService<OllamaClient>();
        Ollama.Client.Models.OllamaClientOptions options = serviceProvider.GetRequiredService<Ollama.Client.Models.OllamaClientOptions>();

        Assert.IsNotNull(client);
        Assert.AreEqual(endpoint, options.Endpoint);
    }

    [TestMethod]
    public void AddOllamaChatClient_RegistersModelScopedChatClient()
    {
        ServiceCollection services = [];

        services.AddOllamaClient(new Uri("http://localhost:11434/"));
        services.AddOllamaChatClient("qwen3.5:4b");
        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        OllamaChatClient client = serviceProvider.GetRequiredService<OllamaChatClient>();

        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void AddOllamaGenerateClient_RegistersModelScopedGenerateClient()
    {
        ServiceCollection services = [];

        services.AddOllamaClient(new Uri("http://localhost:11434/"));
        services.AddOllamaGenerateClient("qwen3.5:4b");
        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        OllamaGenerateClient client = serviceProvider.GetRequiredService<OllamaGenerateClient>();

        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void AddOllamaEmbeddingClient_RegistersModelScopedEmbeddingClient()
    {
        ServiceCollection services = [];

        services.AddOllamaClient(new Uri("http://localhost:11434/"));
        services.AddOllamaEmbeddingClient("nomic-embed-text");
        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        OllamaEmbeddingClient client = serviceProvider.GetRequiredService<OllamaEmbeddingClient>();

        Assert.IsNotNull(client);
    }
}
