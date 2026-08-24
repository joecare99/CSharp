using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models.OpenAI;

/// <summary>Represents the paginated model collection returned by `/v1/models`.</summary>
public sealed class OpenAiModelList
{
    [JsonPropertyName("object")]
    public string Object { get; init; } = "list";

    [JsonPropertyName("data")]
    public IReadOnlyList<OpenAiModel> Data { get; init; } = [];
}
