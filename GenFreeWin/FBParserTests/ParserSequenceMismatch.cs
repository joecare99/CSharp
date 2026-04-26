namespace FBParserTests;

/// <summary>
/// Represents the first detected difference between an expected and an actual parser callback sequence.
/// </summary>
internal readonly record struct ParserSequenceMismatch(int Index, ParseResult? Expected, ParseResult? Actual);
