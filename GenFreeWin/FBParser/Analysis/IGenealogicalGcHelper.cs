namespace FBParser.Analysis;

/// <summary>
/// Defines helper functionality for genealogical GC parsing branches.
/// </summary>
internal interface IGenealogicalGcHelper
{
    /// <summary>
    /// Handles a GC date entry.
    /// </summary>
    bool HandleGcDateEntry(string text, ref int position, string individualId, ref int mode, ref int retMode, ref ParserEventType entryType);

    /// <summary>
    /// Handles a GC non-person entry.
    /// </summary>
    bool HandleGcNonPersonEntry(string subString, char actChar, string individualId);

    /// <summary>
    /// Scans for a child event date following a GC child marker.
    /// </summary>
    string ScanForEventDate(string text, int offset);
}
