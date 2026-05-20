using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client;
using Ollama.Protocol.Models;

namespace Ollama.Client.Tests.TestDoubles;

internal sealed class TestOllamaProtocolAdapter : IOllamaProtocolAdapter
{
    public Func<CancellationToken, Task<OllamaTagsResponse>>? GetTagsAsyncHandler { get; init; }

    public Func<OllamaGenerateRequest, CancellationToken, IAsyncEnumerable<OllamaGenerateResponseChunk>>? GenerateStreamingAsyncHandler { get; init; }

    public Func<OllamaChatRequest, CancellationToken, IAsyncEnumerable<OllamaChatResponseChunk>>? ChatStreamingAsyncHandler { get; init; }

    public Func<OllamaEmbedRequest, CancellationToken, Task<OllamaEmbedResponse>>? EmbedAsyncHandler { get; init; }

    public Task<OllamaTagsResponse> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        if (GetTagsAsyncHandler is null)
        {
            throw new InvalidOperationException("No handler was configured for GetTagsAsync.");
        }

        return GetTagsAsyncHandler(cancellationToken);
    }

    public IAsyncEnumerable<OllamaGenerateResponseChunk> GenerateStreamingAsync(OllamaGenerateRequest request, CancellationToken cancellationToken = default)
    {
        if (GenerateStreamingAsyncHandler is null)
        {
            throw new InvalidOperationException("No handler was configured for GenerateStreamingAsync.");
        }

        return GenerateStreamingAsyncHandler(request, cancellationToken);
    }

    public IAsyncEnumerable<OllamaChatResponseChunk> ChatStreamingAsync(OllamaChatRequest request, CancellationToken cancellationToken = default)
    {
        if (ChatStreamingAsyncHandler is null)
        {
            throw new InvalidOperationException("No handler was configured for ChatStreamingAsync.");
        }

        return ChatStreamingAsyncHandler(request, cancellationToken);
    }

    public Task<OllamaEmbedResponse> EmbedAsync(OllamaEmbedRequest request, CancellationToken cancellationToken = default)
    {
        if (EmbedAsyncHandler is null)
        {
            throw new InvalidOperationException("No handler was configured for EmbedAsync.");
        }

        return EmbedAsyncHandler(request, cancellationToken);
    }
}
