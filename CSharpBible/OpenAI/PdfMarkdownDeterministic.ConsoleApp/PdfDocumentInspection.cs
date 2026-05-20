using System.Collections.Generic;

namespace PdfMarkdownDeterministic.ConsoleApp;

public sealed record PdfDocumentInspection(
    bool HasTextOperators,
    bool HasVectorDrawingHints,
    bool HasToUnicodeMap,
    int ImageObjectCount,
    int XObjectReferenceCount,
    int InlineImageMarkerCount,
    IReadOnlyDictionary<string, string> Metadata,
    IReadOnlyList<string> Fonts,
    IReadOnlyList<string> Encodings,
    IReadOnlyList<string> XObjects,
    IReadOnlyList<string> ContentHints,
    IReadOnlyList<PdfObjectSummary> Objects);
