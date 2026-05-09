using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents a request sent to the Ollama chat endpoint.
/// </summary>
public sealed class OllamaChatRequest
{
    /// <summary>
    /// Gets the model name that should handle the conversation.
    /// </summary>
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    /// <summary>
    /// Gets the chat messages that form the conversation.
    /// </summary>
    [JsonPropertyName("messages")]
    public IReadOnlyList<OllamaChatMessage> Messages { get; init; } = Array.Empty<OllamaChatMessage>();

    /// <summary>
    /// Gets a value indicating whether the response should be streamed.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; init; }
}
