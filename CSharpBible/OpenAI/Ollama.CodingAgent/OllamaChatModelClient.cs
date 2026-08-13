using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client;
using Ollama.Client.Models;

namespace Ollama.CodingAgent;

/// <summary>
/// Implements the agent model client using <see cref="OllamaChatClient"/>.
/// </summary>
public sealed class OllamaChatModelClient : IAgentModelClient
{
    private readonly OllamaChatClient _chatClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaChatModelClient"/> class.
    /// </summary>
    /// <param name="chatClient">The underlying chat client.</param>
    public OllamaChatModelClient(OllamaChatClient chatClient)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    }

    /// <inheritdoc />
    public async Task<string> CompleteAsync(IReadOnlyList<AgentMessage> messages, CancellationToken cancellationToken = default)
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

        OllamaChatCompletion completion = await _chatClient.CompleteChatAsync(options, cancellationToken);
        return completion.Content ?? string.Empty;
    }
}
