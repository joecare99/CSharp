using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Protocol.Models;
using Ollama.Protocol.Parsing;

namespace Ollama.Protocol;

/// <summary>
/// Provides low-level protocol access to Ollama HTTP endpoints.
/// </summary>
public sealed class OllamaProtocolClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;
    private readonly OllamaProtocolClientOptions _options;
    private readonly OllamaChatStreamReader _chatStreamReader;
    private readonly OllamaGenerateStreamReader _streamReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaProtocolClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to call the Ollama service.</param>
    /// <param name="options">The protocol client options.</param>
    public OllamaProtocolClient(HttpClient httpClient, OllamaProtocolClientOptions options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _chatStreamReader = new OllamaChatStreamReader(JsonSerializerOptions);
        _streamReader = new OllamaGenerateStreamReader(JsonSerializerOptions);
    }

    /// <summary>
    /// Gets the models reported by the Ollama tags endpoint.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The available model information.</returns>
    public async Task<OllamaTagsResponse> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, new Uri(_options.Endpoint, "/api/tags"));
        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using Stream responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        OllamaTagsResponse? tagsResponse = await JsonSerializer.DeserializeAsync<OllamaTagsResponse>(responseStream, JsonSerializerOptions, cancellationToken);

        return tagsResponse ?? new OllamaTagsResponse();
    }

    /// <summary>
    /// Streams chunks from the Ollama generate endpoint.
    /// </summary>
    /// <param name="request">The generate request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous stream of generate response chunks.</returns>
    public async IAsyncEnumerable<OllamaGenerateResponseChunk> GenerateStreamingAsync(
        OllamaGenerateRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, new Uri(_options.Endpoint, "/api/generate"))
        {
            Content = JsonContent.Create(request, options: JsonSerializerOptions),
        };

        using HttpResponseMessage response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using Stream responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await foreach (OllamaGenerateResponseChunk chunk in _streamReader.ReadChunksAsync(responseStream, cancellationToken))
        {
            yield return chunk;
        }
    }

    /// <summary>
    /// Streams chunks from the Ollama chat endpoint.
    /// </summary>
    /// <param name="request">The chat request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous stream of chat response chunks.</returns>
    public async IAsyncEnumerable<OllamaChatResponseChunk> ChatStreamingAsync(
        OllamaChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, new Uri(_options.Endpoint, "/api/chat"))
        {
            Content = JsonContent.Create(request, options: JsonSerializerOptions),
        };

        using HttpResponseMessage response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using Stream responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await foreach (OllamaChatResponseChunk chunk in _chatStreamReader.ReadChunksAsync(responseStream, cancellationToken))
        {
            yield return chunk;
        }
    }

    /// <summary>
    /// Requests embeddings from the Ollama embed endpoint.
    /// </summary>
    /// <param name="request">The embed request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The embedding response.</returns>
    public async Task<OllamaEmbedResponse> EmbedAsync(OllamaEmbedRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, new Uri(_options.Endpoint, "/api/embed"))
        {
            Content = JsonContent.Create(request, options: JsonSerializerOptions),
        };

        using HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using Stream responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        OllamaEmbedResponse? embedResponse = await JsonSerializer.DeserializeAsync<OllamaEmbedResponse>(responseStream, JsonSerializerOptions, cancellationToken);

        return embedResponse ?? new OllamaEmbedResponse();
    }
}
