using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models.OpenAI;

/// <summary>Specifies reasoning configuration for a thinking model.</summary>
public sealed class OpenAiReasoning
{
    [JsonPropertyName("effort")]
    public required string Effort { get; init; }
}
