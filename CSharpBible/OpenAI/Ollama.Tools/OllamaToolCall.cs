using System.Text.Json.Serialization;

namespace Ollama.Tools;

/// <summary>
/// Represents a requested tool call.
/// </summary>
public sealed class OllamaToolCall
{
    /// <summary>
    /// Gets the tool name to invoke.
    /// </summary>
    [JsonPropertyName("toolName")]
    public required string ToolName { get; init; }

    /// <summary>
    /// Gets the tool input payload.
    /// </summary>
    [JsonPropertyName("input")]
    public string Input { get; init; } = string.Empty;
}
