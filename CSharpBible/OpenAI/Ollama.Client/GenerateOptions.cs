namespace Ollama.Client;

/// <summary>
/// Provides options for text generation requests.
/// </summary>
public sealed class GenerateOptions
{
    /// <summary>
    /// Gets the input prompt.
    /// </summary>
    public string Prompt { get; init; } = string.Empty;
}
