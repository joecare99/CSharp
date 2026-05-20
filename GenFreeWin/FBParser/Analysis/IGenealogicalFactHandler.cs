namespace FBParser.Analysis;

/// <summary>
/// Defines parser-independent handling for genealogical fact fragments after entry analysis.
/// </summary>
internal interface IGenealogicalFactHandler
{
    /// <summary>
    /// Handles an entry that is not a person name and emits the corresponding callbacks through configuration delegates.
    /// </summary>
    /// <param name="subString">The raw entry text.</param>
    /// <param name="individualId">The current individual identifier.</param>
    /// <param name="defaultPlace">The parser default place.</param>
    /// <param name="famRef">An optional family reference.</param>
    /// <param name="previousEntryType">The previously emitted entry type.</param>
    /// <returns>The effective entry type that was emitted.</returns>
    ParserEventType HandleNonPersonEntry(string subString, string individualId, string defaultPlace, string famRef = "", ParserEventType previousEntryType = ParserEventType.evt_Anull);

    /// <summary>
    /// Handles a family fact entry and emits the corresponding callbacks through configuration delegates.
    /// </summary>
    /// <param name="mainFamRef">The main family reference.</param>
    /// <param name="famEntry">The family entry text.</param>
    void HandleFamilyFact(string mainFamRef, string famEntry);
}
