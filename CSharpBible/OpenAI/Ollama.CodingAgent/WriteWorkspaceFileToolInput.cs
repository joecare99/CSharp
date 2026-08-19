using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
namespace Ollama.CodingAgent;

/// <summary>
/// Represents input for the workspace file-write tool.
/// </summary>
public sealed class WriteWorkspaceFileToolInput
{
    /// <summary>
    /// Gets or sets the relative file path.
    /// </summary>
    public required string RelativePath { get; init; }

    /// <summary>
    /// Gets or sets the file content to write.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether an existing file may be overwritten.
    /// </summary>
    public bool Overwrite { get; init; }
}
