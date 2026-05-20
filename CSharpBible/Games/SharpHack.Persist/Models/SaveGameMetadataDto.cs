using System;

namespace SharpHack.Persist.Models;

/// <summary>
/// Describes save-file metadata used for compatibility, diagnostics, and recovery workflows.
/// </summary>
public sealed class SaveGameMetadataDto
{
    /// <summary>
    /// Gets or sets the logical save-model version.
    /// </summary>
    public int SaveVersion { get; set; } = 1;

    /// <summary>
    /// Gets or sets the UTC timestamp when the save payload was first created.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the save payload was last updated.
    /// </summary>
    public DateTimeOffset UpdatedUtc { get; set; }

    /// <summary>
    /// Gets or sets the optional source application or component identifier.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional diagnostics tag for the payload.
    /// </summary>
    public string DiagnosticsTag { get; set; } = string.Empty;
}
