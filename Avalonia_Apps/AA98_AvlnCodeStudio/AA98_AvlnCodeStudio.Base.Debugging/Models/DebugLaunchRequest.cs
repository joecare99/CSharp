using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.Debugging.Models;

/// <summary>
/// Represents a provider-neutral request to start or attach a debugging session.
/// </summary>
public sealed class DebugLaunchRequest
{
    /// <summary>
    /// Gets or sets the optional launch target path or identifier.
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Gets the optional command-line arguments.
    /// </summary>
    public IList<string> Arguments { get; } = new List<string>();

    /// <summary>
    /// Gets or sets the optional working directory.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the request should attach to an existing process.
    /// </summary>
    public bool AttachToExistingProcess { get; set; }
}
