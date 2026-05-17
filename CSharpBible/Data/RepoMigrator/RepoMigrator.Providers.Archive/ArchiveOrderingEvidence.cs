namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents the explainable evidence used to determine one archive ordering result.
/// </summary>
public sealed class ArchiveOrderingEvidence
{
    /// <summary>
    /// Gets the recorded ordering signals in evaluation order.
    /// </summary>
    public IReadOnlyList<ArchiveOrderingSignal> Signals { get; init; } = Array.Empty<ArchiveOrderingSignal>();
}
