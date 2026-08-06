namespace RnzTrauer.Media;

/// <summary>Captured result of one external process invocation.</summary>
public sealed record ExternalProcessResult(
    int ExitCode,
    string StandardOutput,
    string StandardError);
