using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Providers.Patch.Services;

/// <summary>
/// Provides the first read-only patch-based structured change source for local directory inputs.
/// </summary>
public sealed class DirectoryPatchChangeSetSource : IMigrationChangeSetSource
{
    private static readonly string[] DefaultPatchExtensions = [".patch", ".diff"];
    private readonly string _workspaceRootPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryPatchChangeSetSource"/> class.
    /// </summary>
    /// <param name="workspaceRootPath">The workspace root used to resolve relative directory paths.</param>
    public DirectoryPatchChangeSetSource(string? workspaceRootPath = null)
    {
        _workspaceRootPath = string.IsNullOrWhiteSpace(workspaceRootPath)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(workspaceRootPath);
    }

    /// <inheritdoc/>
    public string Name => "Local directory patch source";

    /// <inheritdoc/>
    public bool CanHandle(MigrationSourceDefinition source)
    {
        if (source.Kind != MigrationSourceKind.PatchCollection)
            return false;

        try
        {
            var patchSource = PatchMigrationSourceDefinition.FromMigrationSourceDefinition(source);
            return patchSource.LocationKind == PatchSourceLocationKind.LocalDirectory;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public Task<ChangeApplicationCapabilities> GetCapabilitiesAsync(MigrationSourceDefinition source, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(source);
        ct.ThrowIfCancellationRequested();

        if (!CanHandle(source))
            throw new NotSupportedException("The supplied source definition is not a local directory patch source.");

        return Task.FromResult(new ChangeApplicationCapabilities
        {
            SupportsStructuredChanges = true,
            SupportsDirectoryChanges = true,
            SupportsTextChanges = true,
            SupportsTextHunks = true,
            SupportsBinaryChanges = true,
            SupportsBinaryPayloadReferences = true,
            SupportsPathRewrites = true,
            SupportedDirectoryChangeKinds =
            [
                MigrationDirectoryChangeKind.Add,
                MigrationDirectoryChangeKind.Delete,
                MigrationDirectoryChangeKind.Rename,
                MigrationDirectoryChangeKind.Copy
            ],
            SupportedChangeKinds =
            [
                MigrationFileChangeKind.Add,
                MigrationFileChangeKind.Modify,
                MigrationFileChangeKind.Delete,
                MigrationFileChangeKind.Rename,
                MigrationFileChangeKind.Copy
            ],
            MaxInlineBinaryPayloadBytes = 0
        });
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<MigrationChangeSet>> GetChangeSetsAsync(MigrationSourceDefinition source, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (!CanHandle(source))
            throw new NotSupportedException("The supplied source definition is not a local directory patch source.");

        ct.ThrowIfCancellationRequested();
        var patchSource = PatchMigrationSourceDefinition.FromMigrationSourceDefinition(source);
        var resolvedDirectoryPath = ResolveDirectoryPath(patchSource.Location);
        if (!Directory.Exists(resolvedDirectoryPath))
            throw new DirectoryNotFoundException($"Patch source directory '{resolvedDirectoryPath}' does not exist.");

        var allowedExtensions = NormalizeExtensions(patchSource.AllowedExtensions);
        var searchOption = patchSource.RecursiveDirectoryScan ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var patchFilePaths = Directory.EnumerateFiles(resolvedDirectoryPath, "*", searchOption)
            .Where(filePath => MatchesAllowedExtension(filePath, allowedExtensions))
            .OrderBy(filePath => filePath, StringComparer.OrdinalIgnoreCase)
            .ThenBy(filePath => filePath, StringComparer.Ordinal)
            .ToArray();

        if (patchFilePaths.Length == 0)
            throw new InvalidOperationException($"Patch source directory '{resolvedDirectoryPath}' does not contain supported patch files.");

        var parser = new PatchChangeSetParser(patchSource.PathRewrites);
        var changeSets = new List<MigrationChangeSet>(patchFilePaths.Length);
        foreach (var patchFilePath in patchFilePaths)
        {
            ct.ThrowIfCancellationRequested();
            var patchText = await File.ReadAllTextAsync(patchFilePath, ct).ConfigureAwait(false);
            changeSets.Add(parser.ParseFile(patchFilePath, patchText));
        }

        return changeSets;
    }

    private string ResolveDirectoryPath(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Patch source location must not be empty.", nameof(location));

        return Path.IsPathFullyQualified(location)
            ? Path.GetFullPath(location)
            : Path.GetFullPath(Path.Combine(_workspaceRootPath, location));
    }

    private static IReadOnlyList<string> NormalizeExtensions(IReadOnlyList<string> allowedExtensions)
    {
        if (allowedExtensions.Count == 0)
            return DefaultPatchExtensions;

        return allowedExtensions
            .Where(extension => !string.IsNullOrWhiteSpace(extension))
            .Select(extension => extension.StartsWith('.') ? extension : $".{extension}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool MatchesAllowedExtension(string filePath, IReadOnlyList<string> allowedExtensions)
        => allowedExtensions.Any(extension => filePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
}
