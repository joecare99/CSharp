using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Models;
using Ollama.Protocol.Models;

namespace Ollama.Client;

/// <summary>
/// Provides model-scoped chat operations.
/// </summary>
public sealed class OllamaChatClient
{
    private readonly string _model;
    private readonly IOllamaProtocolAdapter _protocolAdapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaChatClient"/> class.
    /// </summary>
    /// <param name="protocolClient">The underlying protocol client.</param>
    /// <param name="model">The bound model name.</param>
    public OllamaChatClient(IOllamaProtocolAdapter protocolAdapter, string model)
    {
        _protocolAdapter = protocolAdapter ?? throw new ArgumentNullException(nameof(protocolAdapter));
        _model = string.IsNullOrWhiteSpace(model)
            ? throw new ArgumentException("The model name must not be empty.", nameof(model))
            : model;
    }

    /// <summary>
    /// Gets a buffered chat completion for the provided user message.
    /// </summary>
    /// <param name="message">The user message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The buffered chat completion.</returns>
    public async Task<OllamaChatCompletion> CompleteChatAsync(string message, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return await CompleteChatAsync(
            new ChatCompletionOptions
            {
                Messages =
                [
                    new OllamaClientChatMessage
                    {
                        Role = "user",
                        Content = message,
                    },
                ],
            },
            cancellationToken);
    }

    /// <summary>
    /// Gets a buffered chat completion for the provided options.
    /// </summary>
    /// <param name="options">The chat completion options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The buffered chat completion.</returns>
    public async Task<OllamaChatCompletion> CompleteChatAsync(ChatCompletionOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ValidateMessages(options.Messages);

        StringBuilder contentBuilder = new();
        List<string> thinking = [];

        await foreach (OllamaStreamingChatUpdate update in CompleteChatStreamingAsync(options, cancellationToken))
        {
            if (!string.IsNullOrWhiteSpace(update.Content))
            {
                contentBuilder.Append(update.Content);
            }

            if (!string.IsNullOrWhiteSpace(update.Thinking))
            {
                thinking.Add(update.Thinking);
            }
        }

        return new OllamaChatCompletion
        {
            Content = contentBuilder.ToString(),
            Thinking = thinking,
        };
    }

    /// <summary>
    /// Streams a chat completion for the provided user message.
    /// </summary>
    /// <param name="message">The user message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous stream of chat updates.</returns>
    public async IAsyncEnumerable<OllamaStreamingChatUpdate> CompleteChatStreamingAsync(
        string message,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        await foreach (OllamaStreamingChatUpdate update in CompleteChatStreamingAsync(
            new ChatCompletionOptions
            {
                Messages =
                [
                    new OllamaClientChatMessage
                    {
                        Role = "user",
                        Content = message,
                    },
                ],
            },
            cancellationToken))
        {
            yield return update;
        }
    }

    /// <summary>
    /// Streams a chat completion for the provided options.
    /// </summary>
    /// <param name="options">The chat completion options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous stream of chat updates.</returns>
    public async IAsyncEnumerable<OllamaStreamingChatUpdate> CompleteChatStreamingAsync(
        ChatCompletionOptions options,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ValidateMessages(options.Messages);

        OllamaChatRequest request = new()
        {
            Model = _model,
            Messages = options.Messages
                .Select(static message => new OllamaChatMessage
                {
                    Role = message.Role,
                    Content = message.Content,
                })
                .ToArray(),
            Stream = true,
        };

        await foreach (OllamaChatResponseChunk chunk in _protocolAdapter.ChatStreamingAsync(request, cancellationToken))
        {
            yield return new OllamaStreamingChatUpdate
            {
                Content = chunk.Message?.Content,
                Thinking = chunk.Thinking,
                Done = chunk.Done,
            };
        }
    }

    private static void ValidateMessages(IReadOnlyList<OllamaClientChatMessage> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        if (messages.Count == 0)
        {
            throw new ArgumentException("At least one chat message is required.", nameof(messages));
        }

        foreach (OllamaClientChatMessage message in messages)
        {
            if (string.IsNullOrWhiteSpace(message.Role))
            {
                throw new ArgumentException("Each chat message requires a role.", nameof(messages));
            }

            if (string.IsNullOrWhiteSpace(message.Content))
            {
                throw new ArgumentException("Each chat message requires content.", nameof(messages));
            }
        }
    }
}
