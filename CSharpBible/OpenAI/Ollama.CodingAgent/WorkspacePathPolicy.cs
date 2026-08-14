using System;
using System.IO;

namespace Ollama.CodingAgent;

/// <summary>
/// Enforces workspace-root path boundaries for delegated tools.
/// </summary>
public sealed class WorkspacePathPolicy
{
    private readonly string _workspaceRoot;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkspacePathPolicy"/> class.
    /// </summary>
    /// <param name="workspaceRoot">The workspace root path.</param>
    public WorkspacePathPolicy(string workspaceRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRoot);
        _workspaceRoot = Path.GetFullPath(workspaceRoot);
    }

    /// <summary>
    /// Gets the normalized workspace root path.
    /// </summary>
    public string WorkspaceRoot => _workspaceRoot;

    /// <summary>
    /// Resolves a relative path under the workspace root.
    /// </summary>
    /// <param name="relativePath">The relative path.</param>
    /// <returns>The normalized absolute path.</returns>
    public string ResolveWorkspacePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            relativePath = ".";
        }

        string combinedPath = Path.GetFullPath(Path.Combine(_workspaceRoot, relativePath));
        if (!combinedPath.StartsWith(_workspaceRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Path '{relativePath}' is outside workspace root '{_workspaceRoot}'.");
        }

        return combinedPath;
    }
}
