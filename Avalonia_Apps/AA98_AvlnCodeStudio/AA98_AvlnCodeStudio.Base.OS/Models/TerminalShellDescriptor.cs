using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.OS.Models;

/// <summary>
/// Describes the shell resolved for a terminal session.
/// </summary>
public sealed class TerminalShellDescriptor
{
    /// <summary>
    /// Gets or sets the optional shell display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the optional executable path.
    /// </summary>
    public string? ExecutablePath { get; set; }

    /// <summary>
    /// Gets the resolved shell arguments.
    /// </summary>
    public IList<string> Arguments { get; } = new List<string>();

    /// <summary>
    /// Gets or sets a value indicating whether the shell came from fallback resolution.
    /// </summary>
    public bool IsFallback { get; set; }
}
