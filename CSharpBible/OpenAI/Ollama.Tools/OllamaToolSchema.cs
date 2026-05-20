using System;
using System.Collections.Generic;

namespace Ollama.Tools;

/// <summary>
/// Describes the expected input schema for a tool.
/// </summary>
public sealed class OllamaToolSchema
{
    /// <summary>
    /// Gets the schema summary.
    /// </summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>
    /// Gets the declared parameters.
    /// </summary>
    public IReadOnlyList<OllamaToolParameter> Parameters { get; init; } = Array.Empty<OllamaToolParameter>();
}
