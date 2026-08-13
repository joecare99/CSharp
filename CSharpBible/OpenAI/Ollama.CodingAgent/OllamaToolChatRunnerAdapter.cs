using System;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client;
using Ollama.Client.Models;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent;

/// <summary>
/// Bridges <see cref="OllamaChatClient"/> to <see cref="IOllamaToolChatRunner"/>.
/// </summary>
public sealed class OllamaToolChatRunnerAdapter : IOllamaToolChatRunner
{
    private readonly OllamaChatClient _chatClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaToolChatRunnerAdapter"/> class.
    /// </summary>
    /// <param name="chatClient">The chat client.</param>
    public OllamaToolChatRunnerAdapter(OllamaChatClient chatClient)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    }

    /// <inheritdoc />
    public Task<OllamaChatCompletion> CompleteChatAsync(ChatCompletionOptions options, CancellationToken cancellationToken = default)
        => _chatClient.CompleteChatAsync(options, cancellationToken);
}
