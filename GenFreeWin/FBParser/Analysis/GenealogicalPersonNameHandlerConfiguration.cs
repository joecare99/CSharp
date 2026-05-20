using System;

namespace FBParser.Analysis;

/// <summary>
/// Stores immutable values and parser callbacks used by <see cref="GenealogicalPersonNameHandler"/>.
/// </summary>
internal sealed class GenealogicalPersonNameHandlerConfiguration
{
    /// <summary>
    /// Gets or initializes the abbreviated unknown-name marker.
    /// </summary>
    public required string UnknownShortMarker { get; init; }

    /// <summary>
    /// Gets or initializes the maiden-name marker.
    /// </summary>
    public required string MaidenNameMarker { get; init; }

    /// <summary>
    /// Gets or initializes the academic title markers.
    /// </summary>
    public required string[] AcademicTitleMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the callback used to test title markers.
    /// </summary>
    public required TestForDelegate TestFor { get; init; }

    /// <summary>
    /// Gets or initializes the callback used to guess sex from a given-name fragment.
    /// </summary>
    public required GuessSexOfGivenNameDelegate GuessSexOfGivenName { get; init; }

    /// <summary>
    /// Gets or initializes the callback used to learn sex from a given-name fragment.
    /// </summary>
    public required LearnSexOfGivenNameDelegate LearnSexOfGivenName { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits an individual name.
    /// </summary>
    public required Action<string, int, string> SetIndiName { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits an individual data field.
    /// </summary>
    public required Action<string, ParserEventType, string> SetIndiData { get; init; }

    /// <summary>
    /// Gets or initializes the callback that assigns a family member.
    /// </summary>
    public required Action<string, string, int> SetFamilyMember { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits a family type.
    /// </summary>
    public required Action<string, int, string> SetFamilyType { get; init; }
}
