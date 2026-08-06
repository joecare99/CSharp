using System;
using System.Collections.Generic;

namespace RnzTrauer.Media;

/// <summary>Safe argument-list request for one external process invocation.</summary>
public sealed record ExternalProcessRequest(
    string ExecutablePath,
    IReadOnlyList<string> Arguments,
    string? WorkingDirectory,
    TimeSpan Timeout);
