using System;

namespace FBParser.Analysis;

/// <summary>
/// Stores immutable values and parser callbacks used by <see cref="GenealogicalGcHelper"/>.
/// </summary>
internal sealed class GenealogicalGcHelperConfiguration
{
    /// <summary>
    /// Gets or initializes the birth marker.
    /// </summary>
    public required string BirthMarker { get; init; }

    /// <summary>
    /// Gets or initializes the baptism marker.
    /// </summary>
    public required string BaptismMarker { get; init; }

    /// <summary>
    /// Gets or initializes the death marker.
    /// </summary>
    public required string DeathMarker { get; init; }

    /// <summary>
    /// Gets or initializes the burial marker.
    /// </summary>
    public required string BurialMarker { get; init; }

    /// <summary>
    /// Gets or initializes the fallen marker.
    /// </summary>
    public required string FallenMarker { get; init; }

    /// <summary>
    /// Gets or initializes the missing marker.
    /// </summary>
    public required string MissingMarker { get; init; }

    /// <summary>
    /// Gets or initializes the child-date marker.
    /// </summary>
    public required string ChildDateMarker { get; init; }

    /// <summary>
    /// Gets or initializes the child-date alternate marker.
    /// </summary>
    public required string ChildDateMarkerAlternate { get; init; }

    /// <summary>
    /// Gets or initializes the default place used for GC non-person entries.
    /// </summary>
    public required Func<string> GetDefaultPlace { get; init; }

    /// <summary>
    /// Gets or initializes the umlaut markers.
    /// </summary>
    public required string[] UmlautMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the title markers.
    /// </summary>
    public required string[] TitleMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the callback used to match markers at one-based positions.
    /// </summary>
    public required TestForDelegate TestFor { get; init; }

    /// <summary>
    /// Gets or initializes the callback used for case-insensitive marker checks.
    /// </summary>
    public required Func<string, int, string, bool> TestForCaseInsensitive { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual data.
    /// </summary>
    public required Action<string, ParserEventType, string> SetIndiData { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual place data.
    /// </summary>
    public required Action<string, ParserEventType, string> SetIndiPlace { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual occupation data.
    /// </summary>
    public required Action<string, ParserEventType, string> SetIndiOccu { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual name data.
    /// </summary>
    public required Action<string, int, string> SetIndiName { get; init; }
}
