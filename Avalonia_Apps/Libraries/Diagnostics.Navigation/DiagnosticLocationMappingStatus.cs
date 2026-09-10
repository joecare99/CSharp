namespace Diagnostics.Navigation;

/// <summary>
/// Defines the outcome categories of diagnostic source-location mapping.
/// </summary>
public enum DiagnosticLocationMappingStatus
{
    /// <summary>The diagnostic contains a usable source location.</summary>
    Available = 0,

    /// <summary>The diagnostic has no source path.</summary>
    MissingSourcePath = 1,

    /// <summary>The source path is present but invalid.</summary>
    InvalidSourcePath = 2,

    /// <summary>The source line or column is invalid.</summary>
    InvalidCoordinates = 3,
}
