using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Core.Diagnostics;
using System.Text;

namespace RepoMigrator.Providers.Git;

/// <summary>
/// Materializes normalized structured changes into a working directory and delegates commits to the existing Git target provider.
/// </summary>
public sealed class GitStructuredChangeSetSink : IMigrationChangeSetSink
{
    private readonly IVersionControlProvider _versionControlProvider;
    private readonly string _workspaceRootPath;
    private readonly string _workDirectoryPath;
    private RepositoryEndpoint? _endpoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitStructuredChangeSetSink"/> class.
    /// </summary>
    public GitStructuredChangeSetSink(IVersionControlProvider versionControlProvider, string? workspaceRootPath = null)
    {
        _versionControlProvider = versionControlProvider ?? throw new ArgumentNullException(nameof(versionControlProvider));
        _workspaceRootPath = string.IsNullOrWhiteSpace(workspaceRootPath)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(workspaceRootPath);
        _workDirectoryPath = Path.Combine(_workspaceRootPath, ".RepoMigratorRuntime", "StructuredChangeSink", Guid.NewGuid().ToString("N"));
    }

    /// <inheritdoc />
    public string Name => $"Structured Git sink ({_versionControlProvider.Name})";

    /// <inheritdoc />
    public bool CanHandle(MigrationDestinationDefinition destination)
        => destination is not null
           && destination.Kind == MigrationDestinationKind.Repository
           && string.Equals(destination.Repository?.ProviderKey, GitProvider.ProviderKey, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public Task<ChangeApplicationCapabilities> GetCapabilitiesAsync(MigrationDestinationDefinition destination, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(destination);
        ct.ThrowIfCancellationRequested();

        if (!CanHandle(destination))
            throw new NotSupportedException("The supplied destination definition is not a Git-backed repository destination.");

        return Task.FromResult(new ChangeApplicationCapabilities
        {
            SupportsStructuredChanges = true,
            SupportsMaterializedWorkdirFallback = true,
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
                MigrationFileChangeKind.Copy,
                MigrationFileChangeKind.ModeChange
            ],
            SupportsPathRewrites = true,
            SupportsTextChanges = true,
            SupportsTextHunks = true,
            SupportsBinaryChanges = true,
            SupportsBinaryPayloadReferences = true,
            MaxInlineBinaryPayloadBytes = 1024 * 1024
        });
    }

    /// <inheritdoc />
    public async Task InitializeAsync(MigrationDestinationDefinition destination, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!CanHandle(destination))
            throw new NotSupportedException("The supplied destination definition is not a Git-backed repository destination.");

        _endpoint = destination.Repository;
        Directory.CreateDirectory(_workDirectoryPath);
        await _versionControlProvider.InitializeTargetAsync(_endpoint!, emptyInit: true, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ApplyChangeSetAsync(MigrationChangeSet changeSet, IMigrationProgress progress, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(changeSet);
        ArgumentNullException.ThrowIfNull(progress);
        EnsureInitialized();
        ct.ThrowIfCancellationRequested();

        ApplyDirectoryChanges(changeSet.DirectoryChanges);
        ApplyFileChanges(changeSet.FileChanges);

        await _versionControlProvider.CommitSnapshotAsync(
            _workDirectoryPath,
            new CommitMetadata
            {
                Message = changeSet.Message,
                AuthorName = string.IsNullOrWhiteSpace(changeSet.AuthorName) ? "Structured migration" : changeSet.AuthorName,
                AuthorEmail = changeSet.AuthorEmail,
                Timestamp = changeSet.Timestamp,
                TargetBranch = ResolveTargetReference(changeSet),
                ExpectedChangedPathCount = changeSet.FileChanges.Count,
                VerifyChangedPathCount = false
            },
            progress,
            ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task FinalizeAsync(CancellationToken ct)
    {
        EnsureInitialized();
        return _versionControlProvider.FlushAsync(ct);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        ResetReadOnlyAttributes(_workDirectoryPath);
        await _versionControlProvider.DisposeAsync().ConfigureAwait(false);
        if (Directory.Exists(_workDirectoryPath))
            Directory.Delete(_workDirectoryPath, recursive: true);
    }

    private void ApplyDirectoryChanges(IReadOnlyList<MigrationDirectoryChange> directoryChanges)
    {
        foreach (var directoryChange in directoryChanges)
        {
            switch (directoryChange.Kind)
            {
                case MigrationDirectoryChangeKind.Add:
                    Directory.CreateDirectory(GetAbsolutePath(directoryChange.PathAfter));
                    break;
                case MigrationDirectoryChangeKind.Delete:
                    DeleteDirectoryIfExists(GetAbsolutePath(directoryChange.PathBefore));
                    break;
                case MigrationDirectoryChangeKind.Rename:
                    MoveDirectory(directoryChange.PathBefore, directoryChange.PathAfter, overwriteTarget: true);
                    break;
                case MigrationDirectoryChangeKind.Copy:
                    CopyDirectory(directoryChange.PathBefore, directoryChange.PathAfter);
                    break;
                default:
                    throw new NotSupportedException($"The directory change kind '{directoryChange.Kind}' is not supported by the Git structured sink.");
            }
        }
    }

    private void ApplyFileChanges(IReadOnlyList<MigrationFileChange> fileChanges)
    {
        foreach (var fileChange in fileChanges)
        {
            switch (fileChange.Kind)
            {
                case MigrationFileChangeKind.Add:
                case MigrationFileChangeKind.Modify:
                    WriteFile(fileChange.PathAfter, fileChange);
                    break;
                case MigrationFileChangeKind.Delete:
                    DeleteFileIfExists(GetAbsolutePath(fileChange.PathBefore));
                    break;
                case MigrationFileChangeKind.Rename:
                    ApplyRename(fileChange);
                    break;
                case MigrationFileChangeKind.Copy:
                    ApplyCopy(fileChange);
                    break;
                case MigrationFileChangeKind.ModeChange:
                    ApplyModeChange(fileChange);
                    break;
                default:
                    throw new NotSupportedException($"The file change kind '{fileChange.Kind}' is not supported by the Git structured sink.");
            }
        }
    }

    private void WriteFile(string relativePath, MigrationFileChange fileChange)
    {
        var absolutePath = GetAbsolutePath(relativePath);
        var directoryPath = Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
            Directory.CreateDirectory(directoryPath);

        if (fileChange.BinaryChange is { } binaryChange)
        {
            File.WriteAllBytes(absolutePath, ResolveBinaryPayload(binaryChange));
            return;
        }

        var existingText = File.Exists(absolutePath)
            ? File.ReadAllText(absolutePath, Encoding.UTF8)
            : null;
        var text = ResolveTextPayload(existingText, fileChange.TextChange);
        File.WriteAllText(absolutePath, text, Encoding.UTF8);
    }

    private void ApplyRename(MigrationFileChange fileChange)
    {
        var sourcePath = GetAbsolutePath(fileChange.PathBefore);
        var targetPath = GetAbsolutePath(fileChange.PathAfter);

        if (File.Exists(sourcePath))
        {
            MoveFile(fileChange.PathBefore, fileChange.PathAfter, overwriteTarget: true);
        }
        else if (HasMaterializedPayload(fileChange))
        {
            DeleteFileIfExists(targetPath);
        }

        if (HasMaterializedPayload(fileChange))
            WriteFile(fileChange.PathAfter, fileChange);
    }

    private void ApplyCopy(MigrationFileChange fileChange)
    {
        var sourceRelativePath = ResolveCopySourcePath(fileChange);
        var sourcePath = GetAbsolutePath(sourceRelativePath);
        var targetPath = GetAbsolutePath(fileChange.PathAfter);

        if (File.Exists(sourcePath))
        {
            CopyFile(sourceRelativePath, fileChange.PathAfter);
        }
        else if (HasMaterializedPayload(fileChange))
        {
            DeleteFileIfExists(targetPath);
        }

        if (HasMaterializedPayload(fileChange))
            WriteFile(fileChange.PathAfter, fileChange);

        ApplyModeMetadata(fileChange, targetPath);
    }

    private void ApplyModeChange(MigrationFileChange fileChange)
    {
        var targetRelativePath = !string.IsNullOrWhiteSpace(fileChange.PathAfter)
            ? fileChange.PathAfter
            : fileChange.PathBefore;
        var targetPath = GetAbsolutePath(targetRelativePath);
        if (!File.Exists(targetPath))
            return;

        ApplyModeMetadata(fileChange, targetPath);
    }

    private static bool HasMaterializedPayload(MigrationFileChange fileChange)
        => fileChange.TextChange is not null || fileChange.BinaryChange is not null;

    private static string ResolveCopySourcePath(MigrationFileChange fileChange)
        => fileChange.Metadata.TryGetValue(StructuredChangeMetadataKeys.PatchCopyFrom, out var copySourcePath)
           && !string.IsNullOrWhiteSpace(copySourcePath)
            ? copySourcePath
            : fileChange.PathBefore;

    private static string ResolveTextPayload(string? existingText, MigrationTextChange? textChange)
    {
        if (textChange is null || textChange.Hunks.Count == 0)
            return string.Empty;

        var lineEnding = textChange.LineEnding ?? Environment.NewLine;
        if (string.IsNullOrEmpty(existingText))
            return RebuildTextFromAddedHunks(textChange.Hunks);

        var sourceLines = SplitLines(existingText);
        var workingLines = sourceLines.ToList();
        var lineOffset = 0;

        foreach (var hunk in textChange.Hunks)
        {
            var startIndex = Math.Max(hunk.OriginalStartLine - 1 + lineOffset, 0);
            var removedLines = SplitLines(hunk.RemovedText);
            var addedLines = SplitLines(hunk.AddedText);

            if (startIndex > workingLines.Count)
                return RebuildTextFromAddedHunks(textChange.Hunks);

            if (!MatchesLines(workingLines, startIndex, removedLines))
                return RebuildTextFromAddedHunks(textChange.Hunks);

            if (removedLines.Count > 0)
                workingLines.RemoveRange(startIndex, removedLines.Count);

            if (addedLines.Count > 0)
                workingLines.InsertRange(startIndex, addedLines);

            lineOffset += addedLines.Count - removedLines.Count;
        }

        return JoinLines(workingLines, lineEnding, preserveTrailingNewline: existingText.EndsWith(lineEnding, StringComparison.Ordinal) || textChange.Hunks.Any(hunk => hunk.AddedText.EndsWith(lineEnding, StringComparison.Ordinal)));
    }

    private static string RebuildTextFromAddedHunks(IReadOnlyList<MigrationTextHunk> hunks)
    {
        var builder = new StringBuilder();
        foreach (var hunk in hunks)
            builder.Append(hunk.AddedText);

        return builder.ToString();
    }

    private static List<string> SplitLines(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return [];

        var normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
        var endsWithNewline = normalized.EndsWith("\n", StringComparison.Ordinal);
        var lines = normalized.Split('\n').ToList();
        if (endsWithNewline && lines.Count > 0)
            lines.RemoveAt(lines.Count - 1);

        return lines;
    }

    private static bool MatchesLines(IReadOnlyList<string> workingLines, int startIndex, IReadOnlyList<string> expectedLines)
    {
        if (expectedLines.Count == 0)
            return true;

        if (startIndex < 0 || startIndex + expectedLines.Count > workingLines.Count)
            return false;

        for (var i = 0; i < expectedLines.Count; i++)
        {
            if (!string.Equals(workingLines[startIndex + i], expectedLines[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    private static string JoinLines(IReadOnlyList<string> lines, string lineEnding, bool preserveTrailingNewline)
    {
        if (lines.Count == 0)
            return string.Empty;

        var text = string.Join(lineEnding, lines);
        return preserveTrailingNewline
            ? text + lineEnding
            : text;
    }

    private byte[] ResolveBinaryPayload(MigrationBinaryChange binaryChange)
    {
        return binaryChange.PayloadMode switch
        {
            MigrationBinaryPayloadMode.Inline => binaryChange.InlinePayload ?? Array.Empty<byte>(),
            MigrationBinaryPayloadMode.FileReference => ReadBinaryPayloadFromReference(binaryChange.PayloadReference),
            _ => throw new NotSupportedException($"The binary payload mode '{binaryChange.PayloadMode}' is not supported by the Git structured sink.")
        };
    }

    private byte[] ReadBinaryPayloadFromReference(BinaryPayloadReference? payloadReference)
    {
        if (payloadReference is null)
            return Array.Empty<byte>();

        if (payloadReference.Kind != BinaryPayloadReferenceKind.RelativeArtifactPath)
            throw new NotSupportedException($"The binary payload reference kind '{payloadReference.Kind}' is not supported by the Git structured sink.");

        var absolutePath = GetArtifactAbsolutePath(payloadReference.Value);
        return File.Exists(absolutePath)
            ? File.ReadAllBytes(absolutePath)
            : Array.Empty<byte>();
    }

    private void MoveDirectory(string pathBefore, string pathAfter, bool overwriteTarget)
    {
        var sourcePath = GetAbsolutePath(pathBefore);
        var targetPath = GetAbsolutePath(pathAfter);
        if (!Directory.Exists(sourcePath))
            return;

        if (overwriteTarget)
            DeleteDirectoryIfExists(targetPath);

        var targetParentDirectory = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrWhiteSpace(targetParentDirectory))
            Directory.CreateDirectory(targetParentDirectory);

        Directory.Move(sourcePath, targetPath);
    }

    private void CopyDirectory(string pathBefore, string pathAfter)
    {
        var sourcePath = GetAbsolutePath(pathBefore);
        var targetPath = GetAbsolutePath(pathAfter);
        if (!Directory.Exists(sourcePath))
            return;

        Directory.CreateDirectory(targetPath);
        foreach (var sourceDirectory in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(sourceDirectory.Replace(sourcePath, targetPath, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var sourceFile in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
        {
            var targetFile = sourceFile.Replace(sourcePath, targetPath, StringComparison.OrdinalIgnoreCase);
            var targetDirectory = Path.GetDirectoryName(targetFile);
            if (!string.IsNullOrWhiteSpace(targetDirectory))
                Directory.CreateDirectory(targetDirectory);
            File.Copy(sourceFile, targetFile, overwrite: true);
        }
    }

    private void MoveFile(string pathBefore, string pathAfter, bool overwriteTarget)
    {
        var sourcePath = GetAbsolutePath(pathBefore);
        var targetPath = GetAbsolutePath(pathAfter);
        if (!File.Exists(sourcePath))
            return;

        var targetDirectory = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrWhiteSpace(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        if (overwriteTarget && File.Exists(targetPath))
            File.Delete(targetPath);

        File.Move(sourcePath, targetPath);
    }

    private void CopyFile(string pathBefore, string pathAfter)
    {
        var sourcePath = GetAbsolutePath(pathBefore);
        var targetPath = GetAbsolutePath(pathAfter);
        if (!File.Exists(sourcePath))
            return;

        var targetDirectory = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrWhiteSpace(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        File.Copy(sourcePath, targetPath, overwrite: true);
    }

    private void DeleteDirectoryIfExists(string absolutePath)
    {
        if (Directory.Exists(absolutePath))
            Directory.Delete(absolutePath, recursive: true);
    }

    private void DeleteFileIfExists(string absolutePath)
    {
        if (File.Exists(absolutePath))
            File.Delete(absolutePath);
    }

    private static void ApplyModeMetadata(MigrationFileChange fileChange, string absolutePath)
    {
        if (!File.Exists(absolutePath))
            return;

        if (!fileChange.Metadata.TryGetValue(StructuredChangeMetadataKeys.PatchNewMode, out var newMode) || string.IsNullOrWhiteSpace(newMode))
            return;

        var fileAttributes = File.GetAttributes(absolutePath);
        var shouldBeReadOnly = !IsWritableMode(newMode);
        if (shouldBeReadOnly)
            fileAttributes |= FileAttributes.ReadOnly;
        else
            fileAttributes &= ~FileAttributes.ReadOnly;

        File.SetAttributes(absolutePath, fileAttributes);
    }

    private static void ResetReadOnlyAttributes(string rootPath)
    {
        if (!Directory.Exists(rootPath))
            return;

        foreach (var filePath in Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories))
        {
            var fileAttributes = File.GetAttributes(filePath);
            if (fileAttributes.HasFlag(FileAttributes.ReadOnly))
                File.SetAttributes(filePath, fileAttributes & ~FileAttributes.ReadOnly);
        }
    }

    private static bool IsWritableMode(string mode)
    {
        var normalizedMode = mode.Trim();
        if (normalizedMode.Length == 0)
            return true;

        var permissionDigits = normalizedMode.Length > 3
            ? normalizedMode[^3..]
            : normalizedMode;
        if (permissionDigits.Length == 0)
            return true;

        return permissionDigits[0] is '2' or '3' or '6' or '7';
    }

    private string GetAbsolutePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return _workDirectoryPath;

        var normalizedPath = relativePath.Replace('/', Path.DirectorySeparatorChar);
        return Path.GetFullPath(Path.Combine(_workDirectoryPath, normalizedPath));
    }

    private string GetArtifactAbsolutePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return _workDirectoryPath;

        var normalizedPath = relativePath.Replace('/', Path.DirectorySeparatorChar);
        return Path.GetFullPath(Path.Combine(_workspaceRootPath, normalizedPath));
    }

    private string? ResolveTargetReference(MigrationChangeSet changeSet)
    {
        if (changeSet.Metadata.TryGetValue(StructuredChangeMetadataKeys.Origin, out var origin) && !string.IsNullOrWhiteSpace(origin))
            return _endpoint!.BranchOrTrunk;

        return _endpoint!.BranchOrTrunk;
    }

    private void EnsureInitialized()
    {
        if (_endpoint is null)
            throw new InvalidOperationException("The structured Git sink has not been initialized.");
    }
}
