using System.Collections.Generic;

namespace PdfMarkdownDeterministic.ConsoleApp;

public sealed record PdfObjectSummary(
    string ObjectId,
    string Identifier,
    string? Type,
    string? Subtype,
    string? BaseFont,
    string? FontName,
    IReadOnlyList<string> Encodings,
    bool HasStream,
    bool HasTextOperators,
    bool HasVectorDrawingHints,
    bool HasToUnicodeMap,
    IReadOnlyList<string> References,
    string? Filter,
    string? StreamKind,
    string? DecodedStreamContent,
    string? RawStreamPreview,
    string? DecodedStreamPreview,
    IReadOnlyList<string> OperatorSummary,
    IReadOnlyList<string> DrawingHints,
    IReadOnlyList<string> GlyphCandidates)
{
    internal string RawBody { get; init; } = string.Empty;
}
