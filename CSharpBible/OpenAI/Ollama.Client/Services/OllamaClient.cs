using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Protocol.Models;
using Ollama.Protocol.Services;

namespace Ollama.Client.Services;

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
    /// Gets the models currently available at the configured Ollama endpoint.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The available models.</returns>
    public Task<OllamaTagsResponse> GetTagsAsync(CancellationToken cancellationToken = default)
        => _protocolAdapter.GetTagsAsync(cancellationToken);

    /// <summary>
    /// Gets the models currently loaded into Ollama memory.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The running-model response.</returns>
    public Task<OllamaPsResponse> GetRunningModelsAsync(CancellationToken cancellationToken = default)
        => _protocolAdapter.GetRunningModelsAsync(cancellationToken);

    /// <summary>
    /// Polls the models currently loaded into Ollama memory at a regular interval.
    /// </summary>
    /// <param name="pollingInterval">The delay between consecutive requests.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous stream containing the response from each poll.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the polling interval is not positive.</exception>
    public async IAsyncEnumerable<OllamaPsResponse> PollRunningModelsAsync(
        TimeSpan pollingInterval,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (pollingInterval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(pollingInterval), "The polling interval must be positive.");
        }

        while (true)
        {
            yield return await GetRunningModelsAsync(cancellationToken);
            await Task.Delay(pollingInterval, cancellationToken);
        }
    }

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
