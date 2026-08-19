using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents one tool definition sent to the Ollama chat endpoint.
/// </summary>
public sealed class OllamaChatToolDefinition
{
    /// <summary>
    /// Gets the function definition.
    /// </summary>
    [JsonPropertyName("function")]
    public required OllamaChatToolFunctionDefinition Function { get; init; }
}
