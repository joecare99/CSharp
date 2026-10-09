using System.Collections.Generic;
using OFBCreator.Projects.Models;

namespace OFBCreator.Publishing.Models;

/// <summary>
/// Host-neutral options for the OFB (Ortsfamilienbuch) publication pipeline.
/// </summary>
/// <remarks>
/// Represents the source, output, filtering, and rendering settings used by any host to publish a family book.
/// Immutable after creation to support safe JSON deserialization.
/// </remarks>
public sealed class OFBGenerateOptions
{
    /// <summary>
    /// Path to the source GEDCOM file (required).
    /// Resolved by the host before invoking the shared publication pipeline.
    /// </summary>
    public string InputPath { get; init; } = default!;

    /// <summary>
    /// Path to the output OFB document (.docx or .odt) (required).
    /// </summary>
    public string OutputPath { get; init; } = default!;

    /// <summary>
    /// Title of the Ortsfamilienbuch (required).
    /// Shown on the cover page and table of contents.
    /// </summary>
    public string Title { get; init; } = default!;

    /// <summary>
    /// GedCom reference ID for filtering families by a specific place.
    /// When null or empty, all places are included in the output.
    /// </summary>
    public string? PlaceId { get; init; }

    /// <summary>
    /// When true, includes descendants of matched families in the export.
    /// Default is false — only direct families at the specified place are exported.
    /// </summary>
    public bool IncludeDescendants { get; init; }

    /// <summary>
    /// Preface/introduction text shown before the main family bodies section.
    /// May be null — the preface section is optional.
    /// </summary>
    public string? Preface { get; init; }

    /// <summary>
    /// Character explanation/legend (Zeichenerklärung) showing OFB symbols.
    /// Examples: * = birth, ~ = baptism, + = death, = = burial.
    /// May be null — the legend section is optional.
    /// </summary>
    public string? Legend { get; init; }

    /// <summary>
    /// Built-in template name or path to an external JSON entry template.
    /// </summary>
    public string Template { get; init; } = "gc";

    /// <summary>
    /// When true (default), outputs DOCX format using Xceed.Document.NET.
    /// Set to false with --odt to output ODF (.odt) format instead.
    /// </summary>
    public bool UseDocxFormat { get; init; } = true;

    /// <summary>
    /// Determines the file extension for the output document based on selected format.
    /// </summary>
    public string OutputExtension => UseDocxFormat ? "docx" : "odt";

    /// <summary>
    /// Optional explicit data source provider identifier (e.g., 'gedcom', 'winahnen').
    /// When null or empty, the data source is auto-detected by probing with CanRead.
    /// </summary>
    public string? DataSource { get; init; }

    /// <summary>
    /// Ordered, project-persisted transformations applied to a detached export view.
    /// </summary>
    public IReadOnlyList<OFBExportRule> ExportRules { get; init; } = [];

    /// <summary>Evidence threshold used when forming surname groups.</summary>
    public OFBGroupingPolicy GroupingPolicy { get; init; } = new();

    /// <summary>Persisted editorial choices for evidence-scored surname merges.</summary>
    public IReadOnlyList<OFBGroupingDecision> GroupingDecisions { get; init; } = [];
}
