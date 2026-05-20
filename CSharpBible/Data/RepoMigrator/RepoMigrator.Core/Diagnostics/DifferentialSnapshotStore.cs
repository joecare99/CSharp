namespace RepoMigrator.Core.Diagnostics;

/// <summary>
/// Stores binary-exact source and destination files that participate in differential snapshot synchronization.
/// </summary>
public static class DifferentialSnapshotStore
{
    private const string SnapshotRootEnvironmentVariable = "REPO_MIGRATOR_DIFFERENTIAL_SNAPSHOT_ROOT";
    private static readonly AsyncLocal<string?> s_snapshotRootOverride = new();

    /// <summary>
    /// Gets or sets the async-local snapshot root override used by tests or diagnostic hosts.
    /// </summary>
    public static string? SnapshotRootOverride
    {
        get => s_snapshotRootOverride.Value;
        set => s_snapshotRootOverride.Value = value;
    }

    /// <summary>
    /// Starts a new diagnostic operation folder for one differential synchronization pass.
    /// </summary>
    /// <param name="sProviderName">The provider name that owns the synchronization pass.</param>
    /// <returns>The operation folder path.</returns>
    public static string StartOperation(string sProviderName)
    {
        var sRootDirectory = SnapshotRootOverride ?? Environment.GetEnvironmentVariable(SnapshotRootEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(sRootDirectory))
            sRootDirectory = Path.Combine(Path.GetTempPath(), "RepoMigrator", "differential-snapshots");

        var sSafeProviderName = SanitizeSegment(sProviderName);
        var sOperationDirectory = Path.Combine(
            Path.GetFullPath(sRootDirectory),
            sSafeProviderName,
            $"{DateTimeOffset.UtcNow:yyyyMMdd_HHmmss_fffffff}_{Guid.NewGuid():N}");

        Directory.CreateDirectory(sOperationDirectory);
        return sOperationDirectory;
    }

    /// <summary>
    /// Saves the source and destination side for one changed file.
    /// </summary>
    /// <param name="sOperationDirectory">The operation folder returned by <see cref="StartOperation" />.</param>
    /// <param name="sRelativePath">The synchronized relative path.</param>
    /// <param name="sSourcePath">The source file path.</param>
    /// <param name="sDestinationPath">The destination file path.</param>
    public static void SaveChangedFile(string sOperationDirectory, string sRelativePath, string sSourcePath, string sDestinationPath)
    {
        SaveFile(sOperationDirectory, "source", sRelativePath, sSourcePath);
        SaveFile(sOperationDirectory, "destination", sRelativePath, sDestinationPath);
    }

    /// <summary>
    /// Saves the destination side for one removed file.
    /// </summary>
    /// <param name="sOperationDirectory">The operation folder returned by <see cref="StartOperation" />.</param>
    /// <param name="sRelativePath">The synchronized relative path.</param>
    /// <param name="sDestinationPath">The destination file path.</param>
    public static void SaveRemovedFile(string sOperationDirectory, string sRelativePath, string sDestinationPath)
        => SaveFile(sOperationDirectory, "removed", sRelativePath, sDestinationPath);

    /// <summary>
    /// Saves the source side for one added file.
    /// </summary>
    /// <param name="sOperationDirectory">The operation folder returned by <see cref="StartOperation" />.</param>
    /// <param name="sRelativePath">The synchronized relative path.</param>
    /// <param name="sSourcePath">The source file path.</param>
    public static void SaveAddedFile(string sOperationDirectory, string sRelativePath, string sSourcePath)
        => SaveFile(sOperationDirectory, "added", sRelativePath, sSourcePath);

    private static void SaveFile(string sOperationDirectory, string sBucketName, string sRelativePath, string sFilePath)
    {
        if (!File.Exists(sFilePath))
            return;

        var sSnapshotPath = Path.Combine(sOperationDirectory, sBucketName, SanitizeRelativePath(sRelativePath));
        Directory.CreateDirectory(Path.GetDirectoryName(sSnapshotPath)!);
        VerifiedFileCopy.CopyAndVerify(sFilePath, sSnapshotPath);
    }

    private static string SanitizeRelativePath(string sRelativePath)
    {
        var arrSegments = sRelativePath
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
            .Select(SanitizeSegment)
            .ToArray();

        return arrSegments.Length == 0 ? "_" : Path.Combine(arrSegments);
    }

    private static string SanitizeSegment(string sSegment)
    {
        var arrInvalidChars = Path.GetInvalidFileNameChars();
        var arrChars = sSegment
            .Select(ch => arrInvalidChars.Contains(ch) ? '_' : ch)
            .ToArray();

        var sSanitized = new string(arrChars).Trim();
        return string.IsNullOrWhiteSpace(sSanitized) ? "_" : sSanitized;
    }
}
