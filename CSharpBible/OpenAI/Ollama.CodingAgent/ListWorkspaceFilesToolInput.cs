using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
namespace Ollama.CodingAgent;

/// <summary>
/// Represents input for the workspace file-list tool.
/// </summary>
public sealed class ListWorkspaceFilesToolInput
{
    /// <summary>
    /// Gets or sets the relative start path.
    /// </summary>
    public string RelativePath { get; init; } = ".";

    /// <summary>
    /// Gets or sets the maximum number of files to return.
    /// </summary>
    public int MaxFiles { get; init; } = 100;
}
