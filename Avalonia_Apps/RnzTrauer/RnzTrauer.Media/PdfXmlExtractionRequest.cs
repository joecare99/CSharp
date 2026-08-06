using System;

namespace RnzTrauer.Media;

/// <summary>Input paths and safety limits for one PDF-to-XML extraction.</summary>
public sealed record PdfXmlExtractionRequest(
    string PdfPath,
    string ToolPath,
    string XmlOutputPath,
    TimeSpan Timeout,
    long MaxXmlBytes = 10 * 1024 * 1024);
