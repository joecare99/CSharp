using System.CommandLine;

/// <summary>
/// Options for the OFB (Ortsfamilienbuch) generation pipeline.
/// </summary>
/// <remarks>
/// Represents all CLI and config parameters needed to generate a local family book.
/// Immutable after creation to support JSON deserialization safety.
/// </remarks>
public sealed class OFBGenerateOptions
{
    /// <summary>
    /// Path to the source GEDCOM file (required).
    /// OS-agnostic: resolved by FileConfigLoader / DI at runtime.
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
    /// When true (default), outputs DOCX format using Xceed.Document.NET.
    /// Set to false with --odt to output ODF (.odt) format instead.
    /// </summary>
    public bool UseDocxFormat { get; init; } = true;

    /// <summary>
    /// Determines the file extension for the output document based on selected format.
    /// </summary>
    public string OutputExtension => UseDocxFormat ? "docx" : "odt";
}
