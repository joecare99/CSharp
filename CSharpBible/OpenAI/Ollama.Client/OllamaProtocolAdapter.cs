using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Protocol;
using Ollama.Protocol.Models;

namespace Ollama.Client;

/// <summary>
/// Adapts the protocol client for the public client layer.
/// </summary>
public sealed class OllamaProtocolAdapter : IOllamaProtocolAdapter
{
    private readonly OllamaProtocolClient _protocolClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaProtocolAdapter"/> class.
    /// </summary>
    /// <param name="protocolClient">The underlying protocol client.</param>
    public OllamaProtocolAdapter(OllamaProtocolClient protocolClient)
    {
        _protocolClient = protocolClient ?? throw new System.ArgumentNullException(nameof(protocolClient));
    }

    /// <inheritdoc/>
    public Task<OllamaTagsResponse> GetTagsAsync(CancellationToken cancellationToken = default) => _protocolClient.GetTagsAsync(cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<OllamaGenerateResponseChunk> GenerateStreamingAsync(
        OllamaGenerateRequest request,
        CancellationToken cancellationToken = default) => _protocolClient.GenerateStreamingAsync(request, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<OllamaChatResponseChunk> ChatStreamingAsync(
        OllamaChatRequest request,
        CancellationToken cancellationToken = default) => _protocolClient.ChatStreamingAsync(request, cancellationToken);

    /// <inheritdoc/>
    public Task<OllamaEmbedResponse> EmbedAsync(OllamaEmbedRequest request, CancellationToken cancellationToken = default) => _protocolClient.EmbedAsync(request, cancellationToken);
}
