using System;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client;
using Ollama.Client.Models;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools;

/// <summary>
/// Adapts the public chat client for the tool loop.
/// </summary>
public sealed class OllamaToolChatRunner : IOllamaToolChatRunner
{
    private readonly OllamaChatClient _chatClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaToolChatRunner"/> class.
    /// </summary>
    /// <param name="chatClient">The chat client.</param>
    public OllamaToolChatRunner(OllamaChatClient chatClient)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    }

    /// <inheritdoc/>
    public Task<OllamaChatCompletion> CompleteChatAsync(ChatCompletionOptions options, CancellationToken cancellationToken = default) => _chatClient.CompleteChatAsync(options, cancellationToken);
}
