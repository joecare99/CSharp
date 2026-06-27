using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.OS.Models;

/// <summary>
/// Represents a provider-neutral request to start a terminal session.
/// </summary>
public sealed class TerminalSessionStartRequest
{
    /// <summary>
    /// Gets or sets the optional workspace root path.
    /// </summary>
    public string? WorkspaceRootPath { get; set; }

    /// <summary>
    /// Gets or sets the optional working directory for the session.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Gets or sets the optional explicit shell executable path.
    /// </summary>
    public string? ShellPath { get; set; }

    /// <summary>
    /// Gets or sets the optional preferred shell display name.
    /// </summary>
    public string? ShellDisplayName { get; set; }

    /// <summary>
    /// Gets the shell arguments to apply when the session starts.
    /// </summary>
    public IList<string> Arguments { get; } = new List<string>();

    /// <summary>
    /// Gets the environment variable overrides to apply to the session.
    /// </summary>
    public IDictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>();
}
