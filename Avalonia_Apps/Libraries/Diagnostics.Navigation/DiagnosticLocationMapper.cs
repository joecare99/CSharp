using AppKomponentBaseLib.Diagnostics;
using Code.Navigation;
using System;
using System.IO;

namespace Diagnostics.Navigation;

/// <summary>
/// Maps shared diagnostics to neutral source-code locations.
/// </summary>
public sealed class DiagnosticLocationMapper
{
    /// <summary>
    /// Maps a diagnostic without changing the diagnostic payload.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to map.</param>
    /// <returns>A defined available or unavailable mapping result.</returns>
    public DiagnosticLocationMappingResult Map(Diagnostic diagnostic)
    {
        ArgumentNullException.ThrowIfNull(diagnostic);

        if (string.IsNullOrWhiteSpace(diagnostic.SourcePath))
        {
            return DiagnosticLocationMappingResult.Unavailable(
                DiagnosticLocationMappingStatus.MissingSourcePath,
                "The diagnostic has no source path.");
        }

        var trimmedSourcePath = diagnostic.SourcePath.Trim();
        if (!Path.IsPathRooted(trimmedSourcePath))
        {
            return DiagnosticLocationMappingResult.Unavailable(
                DiagnosticLocationMappingStatus.InvalidSourcePath,
                "The diagnostic source path must be rooted.");
        }

        string sourcePath;
        try
        {
            sourcePath = Path.GetFullPath(trimmedSourcePath);
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return DiagnosticLocationMappingResult.Unavailable(
                DiagnosticLocationMappingStatus.InvalidSourcePath,
                "The diagnostic source path is invalid.");
        }

        if (diagnostic.LineNumber is <= 0 || diagnostic.ColumnNumber is <= 0)
        {
            return DiagnosticLocationMappingResult.Unavailable(
                DiagnosticLocationMappingStatus.InvalidCoordinates,
                "The diagnostic source coordinates must be positive when supplied.");
        }

        try
        {
            return DiagnosticLocationMappingResult.Available(
                new CodeLocation(sourcePath, diagnostic.LineNumber, diagnostic.ColumnNumber));
        }
        catch (ArgumentException)
        {
            return DiagnosticLocationMappingResult.Unavailable(
                DiagnosticLocationMappingStatus.InvalidSourcePath,
                "The diagnostic source path could not be normalized.");
        }
    }
}
