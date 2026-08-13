namespace Ollama.CodingAgent;

/// <summary>
/// Represents input for the dotnet test delegated tool.
/// </summary>
public sealed class RunDotnetTestToolInput
{
    /// <summary>
    /// Gets or sets the relative test project or solution path.
    /// </summary>
    public required string RelativePath { get; init; }

    /// <summary>
    /// Gets or sets the build configuration.
    /// </summary>
    public string Configuration { get; init; } = "Debug";

    /// <summary>
    /// Gets or sets an optional framework selector.
    /// </summary>
    public string? Framework { get; init; }
}
