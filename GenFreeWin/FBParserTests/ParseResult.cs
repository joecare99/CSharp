namespace FBParserTests;

/// <summary>
/// Represents one parser callback result in a Pascal-test compatible form.
/// </summary>
internal readonly record struct ParseResult(string EventType, string Data, string Reference, int SubType);
