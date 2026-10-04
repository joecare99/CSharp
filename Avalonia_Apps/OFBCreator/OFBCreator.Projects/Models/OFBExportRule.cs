namespace OFBCreator.Projects.Models;

/// <summary>
/// A typed, ordered transformation applied only to an export snapshot.
/// </summary>
public sealed class OFBExportRule
{
    /// <summary>Stable identifier used to report validation and application diagnostics.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("D");

    /// <summary>Unique project-wide sequence number used to resolve matching-rule precedence.</summary>
    public int Order { get; set; }

    /// <summary>Whether this rule participates in validation and export processing.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Target category: person, family, or fact.</summary>
    public string TargetKind { get; set; } = string.Empty;

    /// <summary>Provider-qualified external record identifier, never a display name.</summary>
    public string TargetId { get; set; } = string.Empty;

    /// <summary>Constrained operation such as include, exclude, replace, redact, or generalize.</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Allowlisted field name or fact type affected by this rule.</summary>
    public string? Field { get; set; }

    /// <summary>Replacement value or supported generalization policy.</summary>
    public string? Value { get; set; }

    /// <summary>Optional one-based occurrence for repeated facts of the selected type.</summary>
    public int? Occurrence { get; set; }
}
