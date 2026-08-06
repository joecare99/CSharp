namespace RnzTrauer.Media;

/// <summary>Positioned image candidate reported by the PDF XML document.</summary>
public sealed record PdfImageCandidate(
    double X,
    double Y,
    double Width,
    double Height,
    string? Source);
