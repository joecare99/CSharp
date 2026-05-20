namespace Ollama.Tools;

/// <summary>
/// Represents tool metadata for discovery or prompting.
/// </summary>
public sealed class OllamaToolDescriptor
{
    /// <summary>
    /// Gets the tool name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the tool description.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the declared input schema.
    /// </summary>
    public OllamaToolSchema Schema { get; init; } = new();
}
