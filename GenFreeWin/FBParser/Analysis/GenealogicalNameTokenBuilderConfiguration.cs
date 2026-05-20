using System;

namespace FBParser.Analysis;

/// <summary>
/// Stores immutable values and parser callbacks used by <see cref="GenealogicalNameTokenBuilder"/>.
/// </summary>
internal sealed class GenealogicalNameTokenBuilderConfiguration
{
    /// <summary>
    /// Gets or initializes the protect-space marker.
    /// </summary>
    public required string ProtectSpaceMarker { get; init; }

    /// <summary>
    /// Gets or initializes the short unknown marker.
    /// </summary>
    public required string UnknownMarker { get; init; }

    /// <summary>
    /// Gets or initializes the twin marker.
    /// </summary>
    public required string TwinMarker { get; init; }

    /// <summary>
    /// Gets or initializes the alternate separator marker.
    /// </summary>
    public required string Separator2Marker { get; init; }

    /// <summary>
    /// Gets or initializes the umlaut markers.
    /// </summary>
    public required string[] UmlautMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the callback used to test one of multiple markers at a one-based position.
    /// </summary>
    public required TestForDelegate TestFor { get; init; }

    /// <summary>
    /// Gets or initializes the callback used to parse additional fragments.
    /// </summary>
    public required ParseAdditionalDelegate ParseAdditional { get; init; }

    /// <summary>
    /// Gets or initializes the warning callback.
    /// </summary>
    public required Action<string> Warning { get; init; }
}
