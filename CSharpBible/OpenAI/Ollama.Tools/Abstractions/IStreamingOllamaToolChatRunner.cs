using System;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Client.Models;

namespace Ollama.Tools.Abstractions;

/// <summary>
/// Extends the tool chat runner with live thinking-fragment delivery.
/// </summary>
public interface IStreamingOllamaToolChatRunner : IOllamaToolChatRunner
{
    /// <summary>
    /// Requests a chat completion and reports thinking fragments as they arrive.
    /// </summary>
    Task<OllamaChatCompletion> CompleteChatAsync(
        ChatCompletionOptions options,
        Action<string> onThinkingFragment,
        CancellationToken cancellationToken = default);
}