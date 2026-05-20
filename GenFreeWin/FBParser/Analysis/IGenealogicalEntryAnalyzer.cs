namespace FBParser.Analysis;

/// <summary>
/// Defines parser-independent analysis helpers for genealogical free-text entry fragments.
/// </summary>
internal interface IGenealogicalEntryAnalyzer
{
    /// <summary>
    /// Determines the event type encoded by a free-text entry prefix.
    /// </summary>
    /// <param name="subString">The entry text to inspect.</param>
    /// <param name="date">Receives the extracted date fragment when available.</param>
    /// <param name="data">Receives the extracted payload when available.</param>
    /// <returns>The detected event type.</returns>
    ParserEventType GetEntryType(string subString, out string date, out string data);

    /// <summary>
    /// Analyses a generic entry and splits it into event type, data, place, and date fragments.
    /// </summary>
    /// <param name="subString">The mutable entry text fragment.</param>
    /// <param name="defaultPlace">The default place to apply when no explicit place was found.</param>
    /// <param name="currentMode">The current parser mode used for compatibility decisions.</param>
    /// <param name="entryType">Receives the detected entry type.</param>
    /// <param name="data">Receives the extracted data payload.</param>
    /// <param name="place">Receives the extracted place fragment.</param>
    /// <param name="date">Receives the extracted date fragment.</param>
    void AnalyseEntry(ref string subString, string defaultPlace, int currentMode, out ParserEventType entryType, out string data, out string place, out string date);

    /// <summary>
    /// Removes a trailing month token from a place fragment when needed.
    /// </summary>
    /// <param name="place">The mutable place fragment.</param>
    void TrimPlaceByMonth(ref string place);

    /// <summary>
    /// Removes a trailing date modifier from a place fragment when needed.
    /// </summary>
    /// <param name="place">The mutable place fragment.</param>
    void TrimPlaceByModif(ref string place);
}
