namespace FBParser.Analysis;

/// <summary>
/// Defines parser-independent handling for genealogical person-name entries.
/// </summary>
internal interface IGenealogicalPersonNameHandler
{
    /// <summary>
    /// Handles a person-name entry including title, maiden name, family membership, and inferred sex.
    /// </summary>
    /// <param name="personEntry">The raw person entry text.</param>
    /// <param name="mainFamRef">The main family reference.</param>
    /// <param name="personType">The target person type marker.</param>
    /// <param name="mode">The current parser mode.</param>
    /// <param name="lastName">Receives the resolved last name.</param>
    /// <param name="personSex">Receives the resolved sex marker.</param>
    /// <param name="aka">An optional AKA fragment.</param>
    /// <param name="famName">An optional family-name fallback.</param>
    /// <returns>The emitted individual identifier.</returns>
    string HandleAkPersonEntry(string personEntry, string mainFamRef, char personType, int mode, out string lastName, out char personSex, string aka = "", string famName = "");
}
