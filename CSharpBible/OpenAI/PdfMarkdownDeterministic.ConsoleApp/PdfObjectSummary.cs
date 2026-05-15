using System.Collections.Generic;

namespace PdfMarkdownDeterministic.ConsoleApp;

public sealed record PdfObjectSummary(
    string ObjectId,
    string? Type,
    string? Subtype,
    string? BaseFont,
    string? FontName,
    IReadOnlyList<string> Encodings,
    bool HasStream,
    bool HasTextOperators,
    bool HasVectorDrawingHints,
    bool HasToUnicodeMap,
    IReadOnlyList<string> References)
{
    internal string RawBody { get; init; } = string.Empty;
}
