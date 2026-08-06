using System.Collections.Generic;

namespace RnzTrauer.Media;

/// <summary>Text and image candidates extracted from one PDF XML document.</summary>
public sealed record PdfXmlExtractionResult(
    string PdfPath,
    string XmlPath,
    string Text,
    IReadOnlyList<PdfImageCandidate> ImageCandidates,
    string StandardError);
