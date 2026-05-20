namespace Ollama.Tools;

/// <summary>
/// Describes one tool input parameter.
/// </summary>
public sealed class OllamaToolParameter
{
    /// <summary>
    /// Gets the parameter name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the parameter type description.
    /// </summary>
    public string Type { get; init; } = "string";

    /// <summary>
    /// Gets the parameter description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the parameter is required.
    /// </summary>
    public bool Required { get; init; }
}
