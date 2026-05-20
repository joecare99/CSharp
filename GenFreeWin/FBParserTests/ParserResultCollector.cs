using System.Collections.Generic;
using FBParser;

namespace FBParserTests;

/// <summary>
/// Collects parser callbacks into a flat list so the emitted sequence can be asserted in tests.
/// </summary>
internal sealed class ParserResultCollector
{
    private readonly List<ParseResult> _results = [];

    /// <summary>
    /// Gets the collected parser results.
    /// </summary>
    public IReadOnlyList<ParseResult> Results => _results;

    /// <summary>
    /// Attaches all relevant parser callbacks to the supplied parser instance.
    /// </summary>
    /// <param name="parser">The parser to observe.</param>
    public void Attach(FBEntryParser parser)
    {
        parser.OnStartFamily += (_, data, reference, subType) => Add("ParserStartFamily", data, reference, subType);
        parser.OnEntryEnd += (_, data, reference, subType) => Add("ParserEntryEnd", data, reference, subType);
        parser.OnFamilyType += (_, data, reference, subType) => Add("ParserFamilyType", data, reference, subType);
        parser.OnFamilyDate += (_, data, reference, subType) => Add("ParserFamilyDate", data, reference, subType);
        parser.OnFamilyData += (_, data, reference, subType) => Add("ParserFamilyData", data, reference, subType);
        parser.OnFamilyPlace += (_, data, reference, subType) => Add("ParserFamilyPlace", data, reference, subType);
        parser.OnFamilyIndiv += (_, data, reference, subType) => Add("ParserFamilyIndiv", data, reference, subType);
        parser.OnIndiName += (_, data, reference, subType) => Add("ParserIndiName", data, reference, subType);
        parser.OnIndiDate += (_, data, reference, subType) => Add("ParserIndiDate", data, reference, subType);
        parser.OnIndiPlace += (_, data, reference, subType) => Add("ParserIndiPlace", data, reference, subType);
        parser.OnIndiOccu += (_, data, reference, subType) => Add("ParserIndiOccu", data, reference, subType);
        parser.OnIndiRel += (_, data, reference, subType) => Add("ParserIndiRel", data, reference, subType);
        parser.OnIndiRef += (_, data, reference, subType) => Add("ParserIndiRef", data, reference, subType);
        parser.OnIndiData += (_, data, reference, subType) => Add("ParserIndiData", data, reference, subType);
        parser.OnParseMessage += (_, kind, message, reference, mode) => Add(kind switch
        {
            ParseMessageKind.Error => "ParserError!",
            ParseMessageKind.Warning => "ParserWarning",
            _ => "ParserDebugMsg",
        }, message, reference, mode);
    }

    private void Add(string eventType, string data, string reference, int subType)
        => _results.Add(new ParseResult(eventType, data, reference, subType));
}
