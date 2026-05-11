using System;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client;
using Ollama.Client.Models;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools.Tests.TestDoubles;

internal sealed class TestOllamaToolChatRunner : IOllamaToolChatRunner
{
    public required Func<ChatCompletionOptions, CancellationToken, Task<OllamaChatCompletion>> CompleteChatAsyncHandler { get; init; }

    public Task<OllamaChatCompletion> CompleteChatAsync(ChatCompletionOptions options, CancellationToken cancellationToken = default) => CompleteChatAsyncHandler(options, cancellationToken);
}
