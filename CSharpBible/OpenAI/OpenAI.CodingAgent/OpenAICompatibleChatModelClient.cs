using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.CodingAgent;

namespace OpenAI.CodingAgent;

/// <summary>
/// Calls an OpenAI-compatible JSON chat-completions endpoint.
/// </summary>
public sealed class OpenAICompatibleChatModelClient : IThinkingAgentModelClient, IAgentProviderClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenAICompatibleClientOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAICompatibleChatModelClient"/> class.
    /// </summary>
    public OpenAICompatibleChatModelClient(HttpClient httpClient, OpenAICompatibleClientOptions options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public AgentProviderCapabilities Capabilities => new()
    {
        ProviderName = "openai-compatible",
        Model = _options.Model,
        SupportsStreaming = false,
        SupportsToolCalls = false,
        SupportsThinking = false,
    };

    /// <inheritdoc />
    public async Task<string> CompleteAsync(IReadOnlyList<AgentMessage> messages, CancellationToken cancellationToken = default)
        => (await CompleteDetailedAsync(messages, cancellationToken)).Content;

    /// <inheritdoc />
    public async Task<AgentCompletion> CompleteDetailedAsync(
        IReadOnlyList<AgentMessage> messages,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages);
        if (messages.Count == 0)
        {
            throw new ArgumentException("At least one message is required.", nameof(messages));
        }

        using HttpRequestMessage request = new(HttpMethod.Post, BuildCompletionEndpoint(_options.Endpoint))
        {
            Content = JsonContent.Create(new
            {
                model = _options.Model,
                messages = messages.Select(static message => new
                {
                    role = message.Role,
                    content = message.Content,
                }),
                stream = false,
            }),
        };

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        }

        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        using JsonDocument document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty("choices", out JsonElement choices)
            || choices.ValueKind != JsonValueKind.Array
            || choices.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("The OpenAI-compatible response did not contain a choice.");
        }

        JsonElement message = choices[0].GetProperty("message");
        string content = message.TryGetProperty("content", out JsonElement contentElement)
            ? contentElement.GetString() ?? string.Empty
            : string.Empty;
        return new AgentCompletion { Content = content };
    }

    private static Uri BuildCompletionEndpoint(Uri endpoint)
    {
        string path = endpoint.AbsolutePath.TrimEnd('/');
        return path.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
            ? new Uri(endpoint, "chat/completions")
            : new Uri(endpoint, "v1/chat/completions");
    }
}
