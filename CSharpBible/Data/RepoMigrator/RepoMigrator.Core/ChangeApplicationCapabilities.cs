namespace RepoMigrator.Core;

/// <summary>
/// Describes which structured-change features a source or destination component can emit or consume safely.
/// </summary>
public sealed class ChangeApplicationCapabilities
{
    /// <summary>
    /// Gets a value indicating whether normalized structured changes are supported at all.
    /// </summary>
    public bool SupportsStructuredChanges { get; init; }

    /// <summary>
    /// Gets a value indicating whether explicit directory-level changes are supported.
    /// </summary>
    public bool SupportsDirectoryChanges { get; init; }

    /// <summary>
    /// Gets a value indicating whether text-based file changes are supported.
    /// </summary>
    public bool SupportsTextChanges { get; init; }

    /// <summary>
    /// Gets a value indicating whether ordered text hunks are supported directly.
    /// </summary>
    public bool SupportsTextHunks { get; init; }

    /// <summary>
    /// Gets a value indicating whether binary changes are supported.
    /// </summary>
    public bool SupportsBinaryChanges { get; init; }

    /// <summary>
    /// Gets a value indicating whether indirect binary payload references are supported.
    /// </summary>
    public bool SupportsBinaryPayloadReferences { get; init; }

    /// <summary>
    /// Gets a value indicating whether explicit path rewrite rules are supported.
    /// </summary>
    public bool SupportsPathRewrites { get; init; }

    /// <summary>
    /// Gets a value indicating whether workdir-based compatibility materialization is supported as a fallback.
    /// </summary>
    public bool SupportsMaterializedWorkdirFallback { get; init; }

    /// <summary>
    /// Gets the supported normalized directory change kinds.
    /// </summary>
    public IReadOnlyList<MigrationDirectoryChangeKind> SupportedDirectoryChangeKinds { get; init; } = Array.Empty<MigrationDirectoryChangeKind>();

    /// <summary>
    /// Gets the supported normalized file change kinds.
    /// </summary>
    public IReadOnlyList<MigrationFileChangeKind> SupportedChangeKinds { get; init; } = Array.Empty<MigrationFileChangeKind>();

    /// <summary>
    /// Gets the maximum recommended inline binary payload size in bytes when one is known.
    /// </summary>
    public int? MaxInlineBinaryPayloadBytes { get; init; }
}
