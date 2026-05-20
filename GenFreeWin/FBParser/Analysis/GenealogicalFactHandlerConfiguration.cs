using System;

namespace FBParser.Analysis;

/// <summary>
/// Stores the delegates and immutable values needed by <see cref="GenealogicalFactHandler"/>.
/// </summary>
internal sealed class GenealogicalFactHandlerConfiguration
{
    /// <summary>
    /// Gets or initializes the ledig marker variants.
    /// </summary>
    public required string[] LedigMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the indefinite article markers.
    /// </summary>
    public required string[] IndefiniteArticles { get; init; }

    /// <summary>
    /// Gets or initializes the title markers.
    /// </summary>
    public required string[] TitleMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the ledig description text.
    /// </summary>
    public required string LedigText { get; init; }

    /// <summary>
    /// Gets or initializes the marriage partner marker.
    /// </summary>
    public required string MarriagePartnerMarker { get; init; }

    /// <summary>
    /// Gets or initializes the callback that analyses a generic entry.
    /// </summary>
    public required AnalyseEntryDelegate AnalyseEntry { get; init; }

    /// <summary>
    /// Gets or initializes the callback that starts a family.
    /// </summary>
    public required Action<string> StartFamily { get; init; }

    /// <summary>
    /// Gets or initializes the callback that assigns a family member.
    /// </summary>
    public required Action<string, string, int> SetFamilyMember { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits a family date.
    /// </summary>
    public required Action<string, ParserEventType, string> SetFamilyDate { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits a family place.
    /// </summary>
    public required Action<string, ParserEventType, string> SetFamilyPlace { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits family data.
    /// </summary>
    public required Action<string, ParserEventType, string> SetFamilyData { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits an individual date.
    /// </summary>
    public required Action<string, ParserEventType, string> SetIndiDate { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits an individual place.
    /// </summary>
    public required Action<string, ParserEventType, string> SetIndiPlace { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual data.
    /// </summary>
    public required Action<string, ParserEventType, string> SetIndiData { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits an individual occupation.
    /// </summary>
    public required Action<string, ParserEventType, string> SetIndiOccu { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits an individual name.
    /// </summary>
    public required Action<string, int, string> SetIndiName { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits a family type.
    /// </summary>
    public required Action<string, int, string> SetFamilyType { get; init; }

    /// <summary>
    /// Gets or initializes the callback that handles an AK person entry.
    /// </summary>
    public required HandleAkPersonEntryDelegate HandleAkPersonEntry { get; init; }

    /// <summary>
    /// Gets or initializes the callback used to consume leading markers.
    /// </summary>
    public required TryConsumeLeadingEntryDelegate TryConsumeLeadingEntry { get; init; }

    /// <summary>
    /// Gets or initializes the callback that checks title markers.
    /// </summary>
    public required TestForDelegate TestFor { get; init; }

    /// <summary>
    /// Gets or initializes the warning callback.
    /// </summary>
    public required Action<string> Warning { get; init; }
}
