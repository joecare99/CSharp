using RepoMigrator.Core;

namespace RepoMigrator.Tools.PipelinedMigration;

/// <summary>
/// Represents a single exported snapshot that is waiting for ordered commit processing.
/// </summary>
public sealed class PipelineWorkItem
{
    /// <summary>
    /// Gets or sets the zero-based position in the requested migration order.
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// Gets or sets the changeset metadata for the exported snapshot.
    /// </summary>
    public ChangeSetInfo ChangeSet { get; init; } = new();

    /// <summary>
    /// Gets or sets the temporary directory that contains the exported snapshot contents.
    /// </summary>
    public string TempDirectory { get; init; } = string.Empty;
}
