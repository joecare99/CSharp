namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Defines known extension-data keys used on archive import plan items.
/// </summary>
public static class ArchiveImportPlanItemExtensionKeys
{
    /// <summary>
    /// Gets the archive driver identifier key.
    /// </summary>
    public const string DriverId = "DriverId";

    /// <summary>
    /// Gets the archive path or URL key.
    /// </summary>
    public const string ArchivePathOrUrl = "ArchivePathOrUrl";

    /// <summary>
    /// Gets the archive file name key.
    /// </summary>
    public const string ArchiveFileName = "ArchiveFileName";

    /// <summary>
    /// Gets the archive base name key.
    /// </summary>
    public const string ArchiveBaseName = "ArchiveBaseName";

    /// <summary>
    /// Gets the extraction-root path key.
    /// </summary>
    public const string ExtractionRootPath = "Extraction.RootPath";

    /// <summary>
    /// Gets the selected commit timestamp key.
    /// </summary>
    public const string CommitTimestamp = "Commit.Timestamp";

    /// <summary>
    /// Gets the selected commit timestamp source key.
    /// </summary>
    public const string CommitTimestampSource = "Commit.TimestampSource";
}
