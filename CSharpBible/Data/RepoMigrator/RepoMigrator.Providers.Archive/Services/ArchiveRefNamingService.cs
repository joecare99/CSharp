namespace RepoMigrator.Providers.Archive.Services;

/// <summary>
/// Derives first-slice default tag and branch names from archive snapshot descriptors.
/// </summary>
public sealed class ArchiveRefNamingService
{
    private static readonly string[] CompoundExtensions = [".tar.gz"];

    /// <summary>
    /// Derives a default tag name for the supplied archive snapshot.
    /// </summary>
    /// <param name="snapshot">The archive snapshot descriptor.</param>
    /// <param name="options">The optional naming options.</param>
    /// <returns>The derived tag name.</returns>
    public string CreateTagName(ArchiveSnapshotDescriptor snapshot, ArchiveRefNamingOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var baseName = GetArchiveBaseName(snapshot);
        return string.Concat(options?.TagPrefix, baseName);
    }

    /// <summary>
    /// Derives a default branch name for the supplied archive snapshot when branch creation is enabled.
    /// </summary>
    /// <param name="snapshot">The archive snapshot descriptor.</param>
    /// <param name="branchOptions">The branch options that control whether and where branches are created.</param>
    /// <param name="namingOptions">The optional naming options.</param>
    /// <returns>The derived branch name, or <see langword="null"/> when branch creation is disabled.</returns>
    public string? CreateBranchName(ArchiveSnapshotDescriptor snapshot, ArchiveBranchOptions? branchOptions = null, ArchiveRefNamingOptions? namingOptions = null)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (branchOptions is null || !branchOptions.CreateBranches)
            return null;

        var baseName = GetArchiveBaseName(snapshot);
        var branchPrefix = namingOptions?.BranchPrefix ?? branchOptions.BranchPrefix;
        return string.Concat(branchPrefix, baseName);
    }

    /// <summary>
    /// Gets the default base name used for first-slice derived refs.
    /// </summary>
    /// <param name="snapshot">The archive snapshot descriptor.</param>
    /// <returns>The archive base name without a supported extension.</returns>
    public string GetArchiveBaseName(ArchiveSnapshotDescriptor snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (!string.IsNullOrWhiteSpace(snapshot.ArchiveBaseName))
            return snapshot.ArchiveBaseName;

        var fileName = !string.IsNullOrWhiteSpace(snapshot.ArchiveFileName)
            ? snapshot.ArchiveFileName
            : Path.GetFileName(snapshot.ArchivePathOrUrl);
        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidOperationException("Archive file name could not be determined for naming.");

        foreach (var compoundExtension in CompoundExtensions)
        {
            if (fileName.EndsWith(compoundExtension, StringComparison.OrdinalIgnoreCase))
                return fileName[..^compoundExtension.Length];
        }

        var lastExtension = Path.GetExtension(fileName);
        return string.IsNullOrEmpty(lastExtension)
            ? fileName
            : fileName[..^lastExtension.Length];
    }
}
