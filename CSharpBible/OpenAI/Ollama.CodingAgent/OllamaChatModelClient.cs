using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;

namespace Ollama.CodingAgent;

/// <summary>
/// Implements the agent model client using <see cref="OllamaChatClient"/>.
/// </summary>
public sealed class OllamaChatModelClient : IStreamingThinkingAgentModelClient, IAgentProviderClient
{
    private readonly OllamaChatClient _chatClient;
    private readonly Func<ChatCompletionOptions, CancellationToken, Task<OllamaChatCompletion>> _completionRunner;
    private readonly Uri _endpoint;
    private readonly ILlmTrafficLogger? _trafficLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaChatModelClient"/> class.
    /// </summary>
    /// <param name="chatClient">The underlying chat client.</param>
    public OllamaChatModelClient(OllamaChatClient chatClient)
        : this(chatClient, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance with a completion execution seam.
    /// </summary>
    /// <param name="chatClient">The chat client.</param>
    /// <param name="completionRunner">The completion runner.</param>
    public OllamaChatModelClient(
        OllamaChatClient chatClient,
        Func<ChatCompletionOptions, CancellationToken, Task<OllamaChatCompletion>>? completionRunner)
        : this(chatClient, completionRunner, null)
    {
    }

    /// <summary>
    /// Initializes an Ollama model client with traffic diagnostics.
    /// </summary>
    public OllamaChatModelClient(
        OllamaChatClient chatClient,
        Uri endpoint,
        ILlmTrafficLogger? trafficLogger)
        : this(chatClient, null, trafficLogger, endpoint)
    {
    }

    private OllamaChatModelClient(
        OllamaChatClient chatClient,
        Func<ChatCompletionOptions, CancellationToken, Task<OllamaChatCompletion>>? completionRunner,
        ILlmTrafficLogger? trafficLogger,
        Uri? endpoint = null)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        _completionRunner = completionRunner ?? _chatClient.CompleteChatAsync;
        _endpoint = endpoint ?? new Uri("ollama://local/chat");
        _trafficLogger = trafficLogger;
    }

    /// <inheritdoc />
    public AgentProviderCapabilities Capabilities => new()
    {
        ProviderName = "ollama",
        Model = _chatClient.Model,
        SupportsStreaming = true,
        SupportsToolCalls = true,
        SupportsThinking = true,
    };

    /// <inheritdoc />
    public async Task<string> CompleteAsync(IReadOnlyList<AgentMessage> messages, CancellationToken cancellationToken = default)
        => (await CompleteDetailedAsync(messages, cancellationToken)).Content;

    /// <inheritdoc />
    public async Task<AgentCompletion> CompleteDetailedAsync(IReadOnlyList<AgentMessage> messages, CancellationToken cancellationToken = default)
        => await CompleteDetailedCoreAsync(messages, null, cancellationToken);

    /// <inheritdoc />
    public async Task<AgentCompletion> CompleteDetailedAsync(
        IReadOnlyList<AgentMessage> messages,
        Action<string> onThinkingFragment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(onThinkingFragment);
        return await CompleteDetailedCoreAsync(messages, onThinkingFragment, cancellationToken);
    }

    private async Task<AgentCompletion> CompleteDetailedCoreAsync(
        IReadOnlyList<AgentMessage> messages,
        Action<string>? onThinkingFragment,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(messages);
        if (messages.Count == 0)
        {
            throw new ArgumentException("At least one message is required.", nameof(messages));
        }

        ChatCompletionOptions options = new()
        {
            Messages = messages
                .Select(static message => new OllamaClientChatMessage
                {
                    Role = message.Role,
                    Content = message.Content,
                })
                .ToArray(),
        };

        string requestPayload = System.Text.Json.JsonSerializer.Serialize(options);
        _trafficLogger?.LogRequest("ollama", _endpoint, "chat.completions", requestPayload);
        try
        {
            OllamaChatCompletion completion;
            if (onThinkingFragment is null)
            {
                completion = await _completionRunner(options, cancellationToken);
            }
            else
            {
                OllamaStreamingChatAggregator aggregator = new(onThinkingFragment);
                await foreach (OllamaStreamingChatUpdate update in _chatClient.CompleteChatStreamingAsync(options, cancellationToken))
                {
                    aggregator.Add(update);
                }

                completion = aggregator.ToCompletion();
            }
            _trafficLogger?.LogResponse(
                "ollama",
                _endpoint,
                "chat.completions",
                null,
                System.Text.Json.JsonSerializer.Serialize(completion));
            return new AgentCompletion
            {
                Content = completion.Content ?? string.Empty,
                Thinking = completion.Thinking,
            };
        }
        catch (Exception exception)
        {
            _trafficLogger?.LogFailure("ollama", _endpoint, "chat.completions", exception, requestPayload);
            throw;
        }
    }
}
