namespace AA98_AvlnCodeStudio.Base.Testing.Models;

/// <summary>
/// A test case reported by a VSTest adapter. Its identity is adapter-defined.
/// </summary>
public sealed record DiscoveredTestCase(
    string Id,
    string ExecutorUri,
    string Source,
    string FullyQualifiedName,
    string DisplayName,
    string? CodeFilePath,
    int? LineNumber);
