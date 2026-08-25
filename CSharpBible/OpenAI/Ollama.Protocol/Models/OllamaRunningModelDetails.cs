using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ollama.Protocol.Models;

/// <summary>
/// Represents metadata for a model currently loaded by Ollama.
/// </summary>
public sealed class OllamaRunningModelDetails
{
    /// <summary>
    /// Gets the parent model name, if one exists.
    /// </summary>
    [JsonPropertyName("parent_model")]
    public string? ParentModel { get; init; }

    /// <summary>
    /// Gets the model format.
    /// </summary>
    [JsonPropertyName("format")]
    public string? Format { get; init; }

    /// <summary>
    /// Gets the primary model family.
    /// </summary>
    [JsonPropertyName("family")]
    public string? Family { get; init; }

    /// <summary>
    /// Gets the model families associated with the model.
    /// </summary>
    [JsonPropertyName("families")]
    public IReadOnlyList<string> Families { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets the approximate parameter count of the model.
    /// </summary>
    [JsonPropertyName("parameter_size")]
    public string? ParameterSize { get; init; }

    /// <summary>
    /// Gets the model quantization level.
    /// </summary>
    [JsonPropertyName("quantization_level")]
    public string? QuantizationLevel { get; init; }
}
