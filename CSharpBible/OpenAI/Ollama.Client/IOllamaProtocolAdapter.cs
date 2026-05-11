using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Protocol.Models;

namespace Ollama.Client;

/// <summary>
/// Defines the protocol operations required by the public client layer.
/// </summary>
public interface IOllamaProtocolAdapter
{
    /// <summary>
    /// Gets the available server models.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The server tags response.</returns>
    Task<OllamaTagsResponse> GetTagsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams generate response chunks.
    /// </summary>
    /// <param name="request">The generate request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The streamed generate chunks.</returns>
    IAsyncEnumerable<OllamaGenerateResponseChunk> GenerateStreamingAsync(
        OllamaGenerateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams chat response chunks.
    /// </summary>
    /// <param name="request">The chat request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The streamed chat chunks.</returns>
    IAsyncEnumerable<OllamaChatResponseChunk> ChatStreamingAsync(
        OllamaChatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests embeddings from the Ollama server.
    /// </summary>
    /// <param name="request">The embed request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The embed response.</returns>
    Task<OllamaEmbedResponse> EmbedAsync(OllamaEmbedRequest request, CancellationToken cancellationToken = default);
}
