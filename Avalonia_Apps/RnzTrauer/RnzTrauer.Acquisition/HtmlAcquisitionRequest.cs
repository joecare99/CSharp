using System;

namespace RnzTrauer.Acquisition;

/// <summary>Input and safety limits for one acquisition operation.</summary>
public sealed record HtmlAcquisitionRequest(
    Uri Source,
    string? ArchivePath = null,
    long MaxBytes = 10 * 1024 * 1024);
