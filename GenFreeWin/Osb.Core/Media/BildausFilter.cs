namespace Osb.Core.Media;

/// <summary>
/// Filter helpers to determine whether a picture record should be processed.
/// Encapsulates the legacy checks for description values and the COND.aus flags.
/// </summary>
public static class BildausFilter
{
    /// <summary>
    /// Returns true when the description marks the picture as a portrait or family picture
    /// in the legacy data model ("Personenbild" or "Familienbild").
    /// </summary>
    public static bool IsPortraitOrFamily(string? description)
    {
        if (string.IsNullOrWhiteSpace(description)) return false;

        return description == "Personenbild" || description == "Familienbild";
    }

    /// <summary>
    /// Determines whether the given entry matches the export options.
    /// Legacy behaviour: portrait/family pictures are controlled by the IncludeDescriptions flag,
    /// all other pictures by IncludePictures.
    /// </summary>
    public static bool MatchesOptions(BildausEntry entry, BildausOptions options)
    {
        if (entry is null || options is null) return false;

        return IsPortraitOrFamily(entry.Description) ? options.IncludeDescriptions : options.IncludePictures;
    }
}
