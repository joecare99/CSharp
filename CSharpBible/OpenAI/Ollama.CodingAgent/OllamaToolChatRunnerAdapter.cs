using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Client.Models;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent;

/// <summary>
/// Bridges <see cref="OllamaChatClient"/> to <see cref="IOllamaToolChatRunner"/>.
/// </summary>
public sealed class OllamaToolChatRunnerAdapter : IStreamingOllamaToolChatRunner
{
    private readonly OllamaChatClient _chatClient;
    private readonly Func<ChatCompletionOptions, CancellationToken, Task<OllamaChatCompletion>> _completionRunner;
    private readonly ILlmTrafficLogger? _trafficLogger;
    private readonly Uri _endpoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaToolChatRunnerAdapter"/> class.
    /// </summary>
    /// <param name="chatClient">The chat client.</param>
    public OllamaToolChatRunnerAdapter(OllamaChatClient chatClient)
        : this(chatClient, null)
    {
    }

    /// <inheritdoc />
    public async Task<OllamaChatCompletion> CompleteChatAsync(
        ChatCompletionOptions options,
        Action<string> onThinkingFragment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(onThinkingFragment);
        string requestPayload = JsonSerializer.Serialize(options);
        _trafficLogger?.LogRequest("ollama", _endpoint, "chat.completions", requestPayload);
        try
        {
            System.Text.StringBuilder content = new();
            List<string> thinking = [];
            List<OllamaChatToolCall> toolCalls = [];
            await foreach (OllamaStreamingChatUpdate update in _chatClient.CompleteChatStreamingAsync(options, cancellationToken))
            {
                if (!string.IsNullOrWhiteSpace(update.Thinking))
                {
                    thinking.Add(update.Thinking!);
                    onThinkingFragment(update.Thinking!);
                }

                if (!string.IsNullOrWhiteSpace(update.Content))
                {
                    content.Append(update.Content);
                }

                toolCalls.AddRange(update.ToolCalls);
            }

            OllamaChatCompletion completion = new()
            {
                Content = content.ToString(),
                Thinking = thinking,
                ToolCalls = toolCalls,
            };
            _trafficLogger?.LogResponse("ollama", _endpoint, "chat.completions", null, JsonSerializer.Serialize(completion));
            return completion;
        }
        catch (Exception exception)
        {
            _trafficLogger?.LogFailure("ollama", _endpoint, "chat.completions", exception, requestPayload);
            throw;
        }
    }

    /// <summary>
    /// Initializes a new instance with a completion execution seam.
    /// </summary>
    /// <param name="chatClient">The chat client.</param>
    /// <param name="completionRunner">The completion runner.</param>
    public OllamaToolChatRunnerAdapter(
        OllamaChatClient chatClient,
        Func<ChatCompletionOptions, CancellationToken, Task<OllamaChatCompletion>>? completionRunner,
        ILlmTrafficLogger? trafficLogger = null,
        Uri? endpoint = null)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        _completionRunner = completionRunner ?? _chatClient.CompleteChatAsync;
        _trafficLogger = trafficLogger;
        _endpoint = endpoint ?? new Uri("ollama://local/chat");
    }

    /// <inheritdoc />
    public Task<OllamaChatCompletion> CompleteChatAsync(ChatCompletionOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        string requestPayload = JsonSerializer.Serialize(options);
        _trafficLogger?.LogRequest("ollama", _endpoint, "chat.completions", requestPayload);
        return CompleteAndLogAsync(options, requestPayload, cancellationToken);
    }

    private async Task<OllamaChatCompletion> CompleteAndLogAsync(
        ChatCompletionOptions options,
        string requestPayload,
        CancellationToken cancellationToken)
    {
        try
        {
            OllamaChatCompletion completion = await _completionRunner(options, cancellationToken);
            _trafficLogger?.LogResponse(
                "ollama",
                _endpoint,
                "chat.completions",
                null,
                JsonSerializer.Serialize(completion));
            return completion;
        }
        catch (Exception exception)
        {
            _trafficLogger?.LogFailure(
                "ollama",
                _endpoint,
                "chat.completions",
                exception,
                requestPayload);
            throw;
        }
    }
}
