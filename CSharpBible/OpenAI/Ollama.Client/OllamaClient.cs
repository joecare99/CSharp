using System;
using System.Net.Http;
using Ollama.Protocol;

namespace Ollama.Client;

/// <summary>
/// Provides access to model-scoped Ollama feature clients.
/// </summary>
public sealed class OllamaClient
{
    private readonly IOllamaProtocolAdapter _protocolAdapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaClient"/> class.
    /// </summary>
    /// <param name="httpClient">The shared HTTP client.</param>
    /// <param name="options">The public client options.</param>
    public OllamaClient(HttpClient httpClient, OllamaClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        OllamaProtocolClient protocolClient = new(httpClient, new OllamaProtocolClientOptions(options.Endpoint));
        _protocolAdapter = new OllamaProtocolAdapter(protocolClient);
    }

    internal OllamaClient(IOllamaProtocolAdapter protocolAdapter)
    {
        _protocolAdapter = protocolAdapter ?? throw new ArgumentNullException(nameof(protocolAdapter));
    }

    /// <summary>
    /// Gets a chat client for the specified model.
    /// </summary>
    /// <param name="model">The target model name.</param>
    /// <returns>A chat client bound to the given model.</returns>
    public OllamaChatClient GetChatClient(string model) => new OllamaChatClient(_protocolAdapter, model);

    /// <summary>
    /// Gets a generate client for the specified model.
    /// </summary>
    /// <param name="model">The target model name.</param>
    /// <returns>A generate client bound to the given model.</returns>
    public OllamaGenerateClient GetGenerateClient(string model) => new OllamaGenerateClient(_protocolAdapter, model);

    /// <summary>
    /// Gets an embedding client for the specified model.
    /// </summary>
    /// <param name="model">The target model name.</param>
    /// <returns>An embedding client bound to the given model.</returns>
    public OllamaEmbeddingClient GetEmbeddingClient(string model) => new OllamaEmbeddingClient(_protocolAdapter, model);
}
