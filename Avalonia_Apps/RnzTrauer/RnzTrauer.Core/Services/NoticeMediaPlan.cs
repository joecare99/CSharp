namespace RnzTrauer.Core.Services;

/// <summary>Read-only media state and automatic-work decisions.</summary>
public sealed record NoticeMediaPlan(
    bool PdfFound,
    bool XmlFound,
    bool PngFound,
    bool UseAlternatePng,
    bool AutoParseText,
    bool AutoLoadXml,
    bool AutoProcessPdf,
    bool AutoLoadImage);
