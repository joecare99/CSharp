using System.Collections.Generic;

namespace Terminal.Core;

/// <summary>
/// Defines the process launch options for a terminal session.
/// </summary>
public sealed class TerminalSessionOptions
{
    /// <summary>
    /// Gets or sets the executable to launch.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the command-line arguments.
    /// </summary>
    public string Arguments { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the working directory.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Gets or sets the initial terminal size.
    /// </summary>
    public TerminalSize InitialSize { get; set; } = new(80, 25);

    /// <summary>
    /// Gets the environment variable overrides.
    /// </summary>
    public IDictionary<string, string?> EnvironmentVariables { get; } = new Dictionary<string, string?>();
}
