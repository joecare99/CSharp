using System.Threading;
using System.Threading.Tasks;
using Ollama.Client;
using Ollama.Client.Models;

namespace Ollama.Tools.Abstractions;

/// <summary>
/// Defines the chat interaction required by the tool loop.
/// </summary>
public interface IOllamaToolChatRunner
{
    /// <summary>
    /// Requests a chat completion for the provided options.
    /// </summary>
    /// <param name="options">The chat completion options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The buffered chat completion.</returns>
    Task<OllamaChatCompletion> CompleteChatAsync(ChatCompletionOptions options, CancellationToken cancellationToken = default);
}
