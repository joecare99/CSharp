namespace Workbench.Builder.Core.Models.Inspection;

/// <summary>
/// Defines the supported host-facing output formats for a project inspection result.
/// </summary>
public enum ProjectInspectionOutputFormat
{
    /// <summary>
    /// Emits a human-readable plain-text report.
    /// </summary>
    PlainText,

    /// <summary>
    /// Emits a JSON document suitable for tooling and automation.
    /// </summary>
    Json,
}
