using System;
using Workbench.Builder.Core.Models.Diagnostics;

namespace Workbench.Builder.Core.Services.Formatting;

/// <summary>
/// Formats structured build diagnostics into location-aware plain-text messages.
/// </summary>
public static class BuildDiagnosticTextFormatter
{
    /// <summary>
    /// Formats the specified diagnostic into an IDE-friendly plain-text representation.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to format.</param>
    /// <returns>The formatted diagnostic text.</returns>
    public static string Format(BuildDiagnostic diagnostic)
    {
        ArgumentNullException.ThrowIfNull(diagnostic);

        string severity = FormatSeverity(diagnostic.Severity);
        string locationPrefix = FormatLocationPrefix(diagnostic);
        return string.IsNullOrWhiteSpace(locationPrefix)
            ? $"{severity} {diagnostic.Code}: {diagnostic.Message}"
            : $"{locationPrefix}: {severity} {diagnostic.Code}: {diagnostic.Message}";
    }

    private static string FormatSeverity(BuildDiagnosticSeverity severity)
    {
        return severity switch
        {
            BuildDiagnosticSeverity.Error => "error",
            BuildDiagnosticSeverity.Warning => "warning",
            _ => "info",
        };
    }

    private static string FormatLocationPrefix(BuildDiagnostic diagnostic)
    {
        if (string.IsNullOrWhiteSpace(diagnostic.FilePath))
        {
            return string.Empty;
        }

        if (diagnostic.Line.HasValue && diagnostic.Column.HasValue)
        {
            return $"{diagnostic.FilePath}({diagnostic.Line.Value},{diagnostic.Column.Value})";
        }

        if (diagnostic.Line.HasValue)
        {
            return $"{diagnostic.FilePath}({diagnostic.Line.Value})";
        }

        return diagnostic.FilePath;
    }
}
