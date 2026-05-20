namespace FBParser.Analysis;

/// <summary>
/// Defines parser-independent token building for genealogical person-name fragments.
/// </summary>
internal interface IGenealogicalNameTokenBuilder
{
    /// <summary>
    /// Builds a name token fragment from the current text position.
    /// </summary>
    /// <param name="text">The source text.</param>
    /// <param name="offset">The mutable one-based offset.</param>
    /// <param name="charCount">The mutable character counter for the current token.</param>
    /// <param name="subString">The mutable current token text.</param>
    /// <param name="additional">Receives the parsed additional fragment.</param>
    /// <returns><see langword="true"/> when token building may continue; otherwise <see langword="false"/>.</returns>
    bool BuildNameToken(string text, ref int offset, ref int charCount, ref string subString, out string additional);

    /// <summary>
    /// Processes a built name token and maps parsed additional fragments to parser-facing state.
    /// </summary>
    /// <param name="text">The source text.</param>
    /// <param name="offset">The mutable one-based offset.</param>
    /// <param name="subString">The mutable current token text.</param>
    /// <param name="data">The mutable additional data accumulator.</param>
    /// <param name="charCount">The mutable token character counter.</param>
    /// <param name="aka">The mutable AKA fragment.</param>
    /// <param name="addEvent">The mutable add-event marker.</param>
    /// <returns><see langword="true"/> when the current name token is complete; otherwise <see langword="false"/>.</returns>
    bool BuildName(string text, ref int offset, ref string subString, ref string data, ref int charCount, ref string? aka, ref ParserEventType addEvent);
}
