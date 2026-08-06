using System;

namespace RnzTrauer.Acquisition;

/// <summary>Raw acquired content and its resulting archive location.</summary>
public sealed record HtmlAcquisitionResult(
    Uri Source,
    byte[] Content,
    string? MediaType,
    string? ArchivedPath);
