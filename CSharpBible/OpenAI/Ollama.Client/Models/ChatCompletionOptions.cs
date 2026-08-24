using System;
using System.Collections.Generic;
using Ollama.Client.Models;

namespace Ollama.Client.Models;

/// <summary>
/// Provides options for chat completion requests.
/// </summary>
public sealed class ChatCompletionOptions
{
    /// <summary>
    /// Gets the chat messages to send.
    /// </summary>
    public IReadOnlyList<OllamaClientChatMessage> Messages { get; init; } = Array.Empty<OllamaClientChatMessage>();

    /// <summary>
    /// Gets the tools available to the model.
    /// </summary>
    public IReadOnlyList<OllamaChatTool> Tools { get; init; } = Array.Empty<OllamaChatTool>();

    /// <summary>
    /// Gets a value indicating whether the model should emit thinking output.
    /// </summary>
    public bool? Think { get; init; }
}
