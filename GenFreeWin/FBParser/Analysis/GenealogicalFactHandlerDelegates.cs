namespace FBParser.Analysis;

/// <summary>
/// Represents the parser-specific callback used to analyse a generic entry.
/// </summary>
/// <param name="subString">The mutable entry text.</param>
/// <param name="entryType">Receives the detected entry type.</param>
/// <param name="data">Receives the extracted data.</param>
/// <param name="place">Receives the extracted place.</param>
/// <param name="date">Receives the extracted date.</param>
internal delegate void AnalyseEntryDelegate(ref string subString, out ParserEventType entryType, out string data, out string place, out string date);

/// <summary>
/// Represents the parser-specific callback used to handle an AK person entry.
/// </summary>
/// <param name="personEntry">The raw person entry text.</param>
/// <param name="mainFamRef">The main family reference.</param>
/// <param name="personType">The target person type marker.</param>
/// <param name="mode">The current parser mode.</param>
/// <param name="lastName">Receives the resolved last name.</param>
/// <param name="personSex">Receives the resolved sex marker.</param>
/// <param name="aka">An optional AKA fragment.</param>
/// <param name="famName">An optional family name fallback.</param>
/// <returns>The emitted individual identifier.</returns>
internal delegate string HandleAkPersonEntryDelegate(string personEntry, string mainFamRef, char personType, int mode, out string lastName, out char personSex, string aka = "", string famName = "");

/// <summary>
/// Represents the parser-specific callback used to consume a leading marker from an entry.
/// </summary>
/// <param name="subString">The mutable entry text.</param>
/// <param name="testStrings">The markers to test.</param>
/// <param name="rest">Receives the remaining text after consumption.</param>
/// <returns><see langword="true"/> when a marker was consumed; otherwise <see langword="false"/>.</returns>
internal delegate bool TryConsumeLeadingEntryDelegate(ref string subString, string[] testStrings, out string rest);

/// <summary>
/// Represents the parser-specific callback used to test one of multiple markers at a one-based position.
/// </summary>
/// <param name="text">The text to inspect.</param>
/// <param name="position">The one-based position.</param>
/// <param name="tests">The test markers.</param>
/// <param name="found">Receives the matched marker index.</param>
/// <returns><see langword="true"/> when a marker matched; otherwise <see langword="false"/>.</returns>
internal delegate bool TestForDelegate(string text, int position, string[] tests, out int found);
