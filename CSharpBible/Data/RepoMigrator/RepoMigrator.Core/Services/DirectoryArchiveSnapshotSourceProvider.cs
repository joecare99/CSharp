using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Core.Services;

/// <summary>
/// Provides first-slice archive source planning for local directory inputs.
/// </summary>
public sealed class DirectoryArchiveSnapshotSourceProvider : IMigrationSourceProvider
{
    private static readonly string[] DefaultArchiveExtensions = [".zip", ".7z", ".tar", ".tar.gz"];
    private readonly string _workspaceRootPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryArchiveSnapshotSourceProvider"/> class.
    /// </summary>
    /// <param name="workspaceRootPath">The workspace root used to resolve relative directory paths.</param>
    public DirectoryArchiveSnapshotSourceProvider(string? workspaceRootPath = null)
    {
        _workspaceRootPath = string.IsNullOrWhiteSpace(workspaceRootPath)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(workspaceRootPath);
    }

    /// <inheritdoc/>
    public string Name => "Local directory archive source";

    /// <inheritdoc/>
    public bool CanHandle(MigrationSourceDefinition source)
        => source.Kind == MigrationSourceKind.ArchiveCollection
            && source.ArchiveSource is not null
            && source.ArchiveSource.LocationKind == ArchiveSourceLocationKind.LocalDirectory;

    /// <inheritdoc/>
    public Task<MigrationSourcePlan> PrepareAsync(MigrationSourceDefinition source, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (!CanHandle(source))
            throw new NotSupportedException("The supplied source definition is not a local directory archive source.");

        ct.ThrowIfCancellationRequested();
        var archiveSource = source.ArchiveSource!;
        var resolvedDirectoryPath = ResolveDirectoryPath(archiveSource.Location);
        if (!Directory.Exists(resolvedDirectoryPath))
            throw new DirectoryNotFoundException($"Archive source directory '{resolvedDirectoryPath}' does not exist.");

        var allowedExtensions = NormalizeExtensions(archiveSource.AllowedExtensions);
        var searchOption = archiveSource.RecursiveDirectoryScan ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var items = Directory.EnumerateFiles(resolvedDirectoryPath, "*", searchOption)
            .Where(filePath => MatchesAllowedExtension(filePath, allowedExtensions))
            .Select(filePath => CreatePlanItem(resolvedDirectoryPath, filePath))
            .OrderBy(item => item.ItemId, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.ItemId, StringComparer.Ordinal)
            .ToArray();

        if (items.Length == 0)
            throw new InvalidOperationException($"Archive source directory '{resolvedDirectoryPath}' does not contain supported archive files.");

        return Task.FromResult(new MigrationSourcePlan
        {
            Source = source,
            Items = items
        });
    }

    private string ResolveDirectoryPath(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Archive source location must not be empty.", nameof(location));

        return Path.IsPathFullyQualified(location)
            ? Path.GetFullPath(location)
            : Path.GetFullPath(Path.Combine(_workspaceRootPath, location));
    }

    private static IReadOnlyList<string> NormalizeExtensions(IReadOnlyList<string> allowedExtensions)
    {
        if (allowedExtensions.Count == 0)
            return DefaultArchiveExtensions;

        return allowedExtensions
            .Where(extension => !string.IsNullOrWhiteSpace(extension))
            .Select(extension => extension.StartsWith('.') ? extension : $".{extension}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool MatchesAllowedExtension(string filePath, IReadOnlyList<string> allowedExtensions)
        => allowedExtensions.Any(extension => filePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase));

    private static MigrationSourcePlanItem CreatePlanItem(string rootDirectoryPath, string filePath)
    {
        var normalizedFilePath = Path.GetFullPath(filePath);
        var relativePath = Path.GetRelativePath(rootDirectoryPath, normalizedFilePath)
            .Replace(Path.DirectorySeparatorChar, '/')
            .Replace(Path.AltDirectorySeparatorChar, '/');

        return new MigrationSourcePlanItem
        {
            ItemId = relativePath,
            SnapshotId = relativePath,
            SourceIdentifier = normalizedFilePath,
            DisplayName = Path.GetFileName(normalizedFilePath)
        };
    }
}
