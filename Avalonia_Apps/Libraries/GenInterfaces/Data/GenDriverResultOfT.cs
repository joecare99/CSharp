using System.Collections.Generic;

namespace GenInterfaces.Data;

/// <summary>
/// Represents the result of a genealogy driver operation that can return a payload.
/// </summary>
/// <typeparam name="TPayload">The payload type.</typeparam>
public sealed class GenDriverResult<TPayload> : GenDriverResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenDriverResult{TPayload}"/> class.
    /// </summary>
    /// <param name="success">A value indicating whether the operation succeeded.</param>
    /// <param name="payload">The optional payload returned by the operation.</param>
    /// <param name="diagnostics">The diagnostics emitted by the operation.</param>
    public GenDriverResult(
        bool success,
        TPayload? payload = default,
        IEnumerable<GenDriverDiagnostic>? diagnostics = null)
        : base(success, diagnostics)
    {
        Payload = payload;
    }

    /// <summary>
    /// Gets the optional payload returned by the operation.
    /// </summary>
    public TPayload? Payload { get; }

    /// <summary>
    /// Creates a successful result with a payload.
    /// </summary>
    /// <param name="payload">The payload returned by the operation.</param>
    /// <param name="diagnostics">Optional diagnostics emitted during the operation.</param>
    /// <returns>A successful driver result.</returns>
    public static GenDriverResult<TPayload> Successful(
        TPayload payload,
        IEnumerable<GenDriverDiagnostic>? diagnostics = null)
        => new(true, payload, diagnostics);

    /// <summary>
    /// Creates a failed result with an optional payload.
    /// </summary>
    /// <param name="payload">The optional payload returned by the operation.</param>
    /// <param name="diagnostics">Optional diagnostics emitted during the operation.</param>
    /// <returns>A failed driver result.</returns>
    public static GenDriverResult<TPayload> Failed(
        TPayload? payload = default,
        IEnumerable<GenDriverDiagnostic>? diagnostics = null)
        => new(false, payload, diagnostics);
}
