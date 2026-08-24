using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models.OpenAI;

/// <summary>Represents one embedding vector.</summary>
public sealed class OpenAiEmbedding
{
    [JsonPropertyName("object")]
    public string Object { get; init; } = "embedding";

    [JsonPropertyName("embedding")]
    public IReadOnlyList<float> Embedding { get; init; } = [];

    [JsonPropertyName("index")]
    public int Index { get; init; }
}
