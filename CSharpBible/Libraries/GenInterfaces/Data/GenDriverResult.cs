using System;
using System.Collections.Generic;
using System.Linq;

namespace GenInterfaces.Data;

/// <summary>
/// Represents the result of a genealogy driver operation.
/// </summary>
public class GenDriverResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenDriverResult"/> class.
    /// </summary>
    /// <param name="success">A value indicating whether the operation succeeded.</param>
    /// <param name="diagnostics">The diagnostics emitted by the operation.</param>
    public GenDriverResult(bool success, IEnumerable<GenDriverDiagnostic>? diagnostics = null)
    {
        Success = success;
        Diagnostics = diagnostics?.ToArray() ?? Array.Empty<GenDriverDiagnostic>();
    }

    /// <summary>
    /// Gets a value indicating whether the driver operation succeeded.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the diagnostics emitted by the driver operation.
    /// </summary>
    public IReadOnlyList<GenDriverDiagnostic> Diagnostics { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="diagnostics">Optional diagnostics emitted during the operation.</param>
    /// <returns>A successful driver result.</returns>
    public static GenDriverResult Successful(IEnumerable<GenDriverDiagnostic>? diagnostics = null)
        => new(true, diagnostics);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="diagnostics">Optional diagnostics emitted during the operation.</param>
    /// <returns>A failed driver result.</returns>
    public static GenDriverResult Failed(IEnumerable<GenDriverDiagnostic>? diagnostics = null)
        => new(false, diagnostics);
}
