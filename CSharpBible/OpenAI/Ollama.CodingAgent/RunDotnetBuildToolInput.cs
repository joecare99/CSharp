namespace Ollama.CodingAgent;

/// <summary>
/// Represents input for the dotnet build delegated tool.
/// </summary>
public sealed class RunDotnetBuildToolInput
{
    /// <summary>
    /// Gets or sets the relative solution or project path.
    /// </summary>
    public required string RelativePath { get; init; }

    /// <summary>
    /// Gets or sets the build configuration.
    /// </summary>
    public string Configuration { get; init; } = "Debug";
}
