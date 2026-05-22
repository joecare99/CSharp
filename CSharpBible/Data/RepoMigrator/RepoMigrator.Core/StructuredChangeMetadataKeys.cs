namespace RepoMigrator.Core;

/// <summary>
/// Provides shared metadata keys used by the provider-agnostic structured-change model.
/// </summary>
public static class StructuredChangeMetadataKeys
{
    /// <summary>
    /// Gets the metadata key used to record the selected execution-path decision.
    /// </summary>
    public const string ExecutionPathDecision = nameof(MigrationExecutionPathSelection) + "." + nameof(MigrationExecutionPathSelection.Kind);

    /// <summary>
    /// Gets the metadata key used to preserve the logical source-kind hint.
    /// </summary>
    public const string SourceKind = "SourceKind";

    /// <summary>
    /// Gets the metadata key used to preserve the logical origin hint of a normalized change.
    /// </summary>
    public const string Origin = "Origin";

    /// <summary>
    /// Gets the metadata key used to preserve the patch old file mode when one was parsed.
    /// </summary>
    public const string PatchOldMode = "PatchOldMode";

    /// <summary>
    /// Gets the metadata key used to preserve the patch new file mode when one was parsed.
    /// </summary>
    public const string PatchNewMode = "PatchNewMode";

    /// <summary>
    /// Gets the metadata key used to preserve the patch copy-source hint when one was parsed.
    /// </summary>
    public const string PatchCopyFrom = "PatchCopyFrom";

    /// <summary>
    /// Gets the metadata key used to preserve the patch copy-target hint when one was parsed.
    /// </summary>
    public const string PatchCopyTo = "PatchCopyTo";
}
