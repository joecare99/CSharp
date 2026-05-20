namespace FBParser.Analysis;

/// <summary>
/// Represents the callback used to parse additional text enclosed in parentheses.
/// </summary>
/// <param name="text">The source text.</param>
/// <param name="position">The mutable one-based position.</param>
/// <param name="output">Receives the parsed additional text.</param>
/// <returns><see langword="true"/> when an additional fragment was parsed; otherwise <see langword="false"/>.</returns>
internal delegate bool ParseAdditionalDelegate(string text, ref int position, out string output);
