using Code.Navigation;
using System;

namespace Diagnostics.Navigation;

/// <summary>
/// Describes the result of mapping a diagnostic to a neutral code location.
/// </summary>
public sealed class DiagnosticLocationMappingResult
{
    private DiagnosticLocationMappingResult(
        CodeLocation? location,
        DiagnosticLocationMappingStatus status,
        string? reason)
    {
        Location = location;
        Status = status;
        Reason = reason;
    }

    /// <summary>Gets the mapped location, when available.</summary>
    public CodeLocation? Location { get; }

    /// <summary>Gets the mapping status.</summary>
    public DiagnosticLocationMappingStatus Status { get; }

    /// <summary>Gets the reason when no location is available.</summary>
    public string? Reason { get; }

    /// <summary>Gets whether a usable location was created.</summary>
    public bool IsAvailable => Status == DiagnosticLocationMappingStatus.Available;

    /// <summary>Creates a successful mapping result.</summary>
    public static DiagnosticLocationMappingResult Available(CodeLocation location)
    {
        ArgumentNullException.ThrowIfNull(location);
        return new(location, DiagnosticLocationMappingStatus.Available, null);
    }

    /// <summary>Creates an unavailable mapping result.</summary>
    public static DiagnosticLocationMappingResult Unavailable(
        DiagnosticLocationMappingStatus status,
        string reason)
    {
        if (status == DiagnosticLocationMappingStatus.Available)
        {
            throw new ArgumentException("An available result requires a location.", nameof(status));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("A mapping reason is required.", nameof(reason));
        }

        return new(null, status, reason);
    }
}
