using System;

namespace FBParser.Analysis;

/// <summary>
/// Stores immutable genealogical tokens and validation callbacks used by <see cref="GenealogicalEntryAnalyzer"/>.
/// </summary>
internal sealed class GenealogicalEntryAnalyzerConfiguration
{
    /// <summary>
    /// Gets or initializes the protect-space marker.
    /// </summary>
    public required string ProtectSpace { get; init; }

    /// <summary>
    /// Gets or initializes the birth marker.
    /// </summary>
    public required string BirthMarker { get; init; }

    /// <summary>
    /// Gets or initializes the baptism markers.
    /// </summary>
    public required string[] BaptismMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the burial markers.
    /// </summary>
    public required string[] BurialMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the marriage markers.
    /// </summary>
    public required string[] MarriageMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the death markers.
    /// </summary>
    public required string[] DeathMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the stillborn markers.
    /// </summary>
    public required string[] StillbornMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the death wording for fallen entries.
    /// </summary>
    public required string FallenMarker { get; init; }

    /// <summary>
    /// Gets or initializes the divorce marker.
    /// </summary>
    public required string DivorceMarker { get; init; }

    /// <summary>
    /// Gets or initializes the missing-person markers.
    /// </summary>
    public required string[] MissingMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the emigration markers.
    /// </summary>
    public required string[] EmigrationMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the supported date modifiers.
    /// </summary>
    public required string[] DateModifiers { get; init; }

    /// <summary>
    /// Gets or initializes the date modifier used for residence phrases.
    /// </summary>
    public required string SinceDateModifier { get; init; }

    /// <summary>
    /// Gets or initializes the age marker.
    /// </summary>
    public required string AgeMarker { get; init; }

    /// <summary>
    /// Gets or initializes the indefinite article markers.
    /// </summary>
    public required string[] IndefiniteArticles { get; init; }

    /// <summary>
    /// Gets or initializes the definite article markers.
    /// </summary>
    public required string[] DefiniteArticles { get; init; }

    /// <summary>
    /// Gets or initializes the called-as marker.
    /// </summary>
    public required string AkaMarker { get; init; }

    /// <summary>
    /// Gets or initializes the leading wording for AKA phrases.
    /// </summary>
    public required string BecameMarker { get; init; }

    /// <summary>
    /// Gets or initializes the description markers.
    /// </summary>
    public required string[] DescriptionMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the residence markers.
    /// </summary>
    public required string[] ResidenceMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the place markers.
    /// </summary>
    public required string[] PlaceMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the unknown markers.
    /// </summary>
    public required string[] UnknownMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the property markers.
    /// </summary>
    public required string[] PropertyMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the address markers.
    /// </summary>
    public required string[] AddressMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the religion markers.
    /// </summary>
    public required string[] ReligionMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the month names.
    /// </summary>
    public required string[] MonthNames { get; init; }

    /// <summary>
    /// Gets or initializes the uppercase umlaut markers.
    /// </summary>
    public required string[] UpperUmlautMarkers { get; init; }

    /// <summary>
    /// Gets or initializes the generic in-place marker.
    /// </summary>
    public required string InPlaceMarker { get; init; }

    /// <summary>
    /// Gets or initializes the destination-place marker.
    /// </summary>
    public required string ToPlaceMarker { get; init; }

    /// <summary>
    /// Gets or initializes the origin-place marker.
    /// </summary>
    public required string FromPlaceMarker { get; init; }

    /// <summary>
    /// Gets or initializes the month-place marker.
    /// </summary>
    public required string InMonthPlaceMarker { get; init; }

    /// <summary>
    /// Gets or initializes the date modifier marker used for place cleanup.
    /// </summary>
    public required string OnDatePlaceMarker { get; init; }

    /// <summary>
    /// Gets or initializes the marriage partner marker.
    /// </summary>
    public required string MarriagePartnerMarker { get; init; }

    /// <summary>
    /// Gets or initializes the callback that validates date fragments.
    /// </summary>
    public required Func<string, bool> IsValidDate { get; init; }

    /// <summary>
    /// Gets or initializes the callback that validates place fragments.
    /// </summary>
    public required Func<string, bool> IsValidPlace { get; init; }

    /// <summary>
    /// Gets or initializes the warning callback used for compatibility diagnostics.
    /// </summary>
    public required Action<string> Warning { get; init; }
}
