using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        Ollama.Client.OllamaClient client = serviceProvider.GetRequiredService<Ollama.Client.OllamaClient>();
        Ollama.Client.OllamaClientOptions options = serviceProvider.GetRequiredService<Ollama.Client.OllamaClientOptions>();

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

        Ollama.Client.OllamaChatClient client = serviceProvider.GetRequiredService<Ollama.Client.OllamaChatClient>();

        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void AddOllamaGenerateClient_RegistersModelScopedGenerateClient()
    {
        ServiceCollection services = [];

        services.AddOllamaClient(new Uri("http://localhost:11434/"));
        services.AddOllamaGenerateClient("qwen3.5:4b");
        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        Ollama.Client.OllamaGenerateClient client = serviceProvider.GetRequiredService<Ollama.Client.OllamaGenerateClient>();

        Assert.IsNotNull(client);
    }

    [TestMethod]
    public void AddOllamaEmbeddingClient_RegistersModelScopedEmbeddingClient()
    {
        ServiceCollection services = [];

        services.AddOllamaClient(new Uri("http://localhost:11434/"));
        services.AddOllamaEmbeddingClient("nomic-embed-text");
        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        Ollama.Client.OllamaEmbeddingClient client = serviceProvider.GetRequiredService<Ollama.Client.OllamaEmbeddingClient>();

        Assert.IsNotNull(client);
    }
}
