using System;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents a model currently loaded by Ollama.
/// </summary>
public sealed class OllamaRunningModel
{
    /// <summary>
    /// Gets the name of the running model.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Gets the internal model identifier.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; init; }

    /// <summary>
    /// Gets the model size in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; init; }

    /// <summary>
    /// Gets the SHA-256 digest of the model.
    /// </summary>
    [JsonPropertyName("digest")]
    public string? Digest { get; init; }

    /// <summary>
    /// Gets the model metadata.
    /// </summary>
    [JsonPropertyName("details")]
    public OllamaRunningModelDetails? Details { get; init; }

    /// <summary>
    /// Gets the time when the model will be unloaded.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// Gets the VRAM usage in bytes.
    /// </summary>
    [JsonPropertyName("size_vram")]
    public long SizeVram { get; init; }

    /// <summary>
    /// Gets the context length for the running model.
    /// </summary>
    [JsonPropertyName("context_length")]
    public int ContextLength { get; init; }
}
