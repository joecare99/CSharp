/// <summary>
/// Defines how OFB configuration is validated and what output format providers are supported.
/// </summary>
namespace OFBCreator.Abstractions.Models;

using GenInterfaces.Interfaces;
using OFBCreator.Abstractions.Interfaces;

/// <summary>
/// Immutable, validated configuration for the OFB pipeline.
/// Only output formats backed by IUserDocumentFactory are valid.
/// </summary>
public sealed class OFBConfiguration
{
    /// <summary>
    /// Path to the source GEDCOM file (required).
    /// </summary>
    public string InputPath { get; init; } = default!;

    /// <summary>
    /// Path to the output OFB document (required).
    /// </summary>
    public string OutputPath { get; init; } = default!;

    /// <summary>
    /// Title of the Ortsfamilienbuch (required).
    /// </summary>
    public string Title { get; init; } = default!;

    /// <summary>
    /// Optional place ID filter for family selection.
    /// Null or empty means all places are included.
    /// </summary>
    public string? PlaceId { get; init; }

    /// <summary>
    /// Whether to include descendants of matched families.
    /// Default is false — only direct families at the specified place.
    /// </summary>
    public bool IncludeDescendants { get; init; }

    /// <summary>
    /// Optional preface/introduction text for the OFB document.
    /// Null means no preface section is generated.
    /// </summary>
    public string? Preface { get; init; }

    /// <summary>
    /// Optional character explanation / legend (Zeichenerklärung).
    /// Null means no legend section is generated.
    /// </summary>
    public string? Legend { get; init; }

    /// <summary>
    /// Provider that supports creating OFB document formats.
    /// Used to validate supported output types at construction time.
    /// </summary>
    public IUserDocumentFormatProvider? FormatProvider { get; init; }

    /// <summary>
    /// Validation errors collected during configuration validation.
    /// Empty when IsValid is true.
    /// </summary>
    public IReadOnlyList<string> Errors { get; private set; } = new List<string>().AsReadOnly();

    /// <summary>
    /// Whether this configuration passes all required validation rules.
    /// </summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>
    /// Creates and validates an OFBConfiguration from raw parameters.
    /// Returns the configuration if valid, or a collection of validation errors.
    /// Missing input, output or title produce actionable failure messages.
    /// </summary>
    public static OFBConfigurationResult Validate(
        string? inputPath,
        string? outputPath,
        string? title,
        IUserDocumentFormatProvider? formatProvider = null )
    {
        var errors = new List<string>();

        if ( string.IsNullOrWhiteSpace( inputPath ) )
            errors.Add( "Input path is required — the source GEDCOM file must be specified." );

        if ( string.IsNullOrWhiteSpace( outputPath ) )
            errors.Add( "Output path is required — the destination OFB document path must be specified." );

        if ( string.IsNullOrWhiteSpace( title ) )
            errors.Add( "Title is required — every OFB requires a cover title." );

        if ( errors.Count != 0 && formatProvider != null )
        {
            // When there are config errors, we can't fully validate the provider.
            // Just check that it supports a common default format.
            const string DEFAULT_FORMAT = "docx";
            if ( !formatProvider.IsFormatSupported( DEFAULT_FORMAT ) )
                errors.Add( $"Output format '{DEFAULT_FORMAT}' not supported by provider '{formatProvider.GetType().Name}'." );
        }

        var config = new OFBConfiguration
        {
            InputPath = inputPath ?? string.Empty,
            OutputPath = outputPath ?? string.Empty,
            Title = title ?? string.Empty,
            FormatProvider = formatProvider,
        };

        if ( errors.Count != 0 )
        {
            config.Errors = errors.AsReadOnly();
            return new OFBConfigurationResult( false, errors );
        }

        return new OFBConfigurationResult( true, config );
    }
}

/// <summary>
/// Result of configuration validation — either a valid configuration or a list of errors.
/// </summary>
public sealed class OFBConfigurationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<string> Errors { get; }
    public OFBConfiguration? Configuration { get; }

    public OFBConfigurationResult( bool isValid, IReadOnlyList<string> errors )
    {
        IsValid = isValid;
        Errors = errors;
        Configuration = null;
    }

    public OFBConfigurationResult( bool isValid, OFBConfiguration config )
    {
        IsValid = isValid;
        Errors = new List<string>().AsReadOnly();
        Configuration = config ?? throw new ArgumentNullException( nameof( config ) );
    }
}
