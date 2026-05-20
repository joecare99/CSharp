using System;
using System.Collections.Generic;
using Ollama.Client.Models;

namespace Ollama.Client;

/// <summary>
/// Provides options for chat completion requests.
/// </summary>
public sealed class ChatCompletionOptions
{
    /// <summary>
    /// Gets the chat messages to send.
    /// </summary>
    public IReadOnlyList<OllamaClientChatMessage> Messages { get; init; } = Array.Empty<OllamaClientChatMessage>();
}
