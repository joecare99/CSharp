using System.Globalization;
using System.Text;
using RepoMigrator.Core;

namespace RepoMigrator.Providers.Patch.Services;

/// <summary>
/// Parses the first read-only subset of unified patch files into normalized structured change sets.
/// </summary>
public sealed class PatchChangeSetParser
{
    private readonly IReadOnlyList<PathRewriteRule> _pathRewrites;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchChangeSetParser"/> class.
    /// </summary>
    /// <param name="pathRewrites">The explicit path rewrites that should be applied while normalizing patch paths.</param>
    public PatchChangeSetParser(IReadOnlyList<PathRewriteRule> pathRewrites)
    {
        _pathRewrites = pathRewrites ?? Array.Empty<PathRewriteRule>();
    }

    /// <summary>
    /// Parses one patch file into a normalized change set.
    /// </summary>
    public MigrationChangeSet ParseFile(string patchFilePath, string patchText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(patchFilePath);
        ArgumentNullException.ThrowIfNull(patchText);

        var lines = patchText.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        var fileChanges = new List<MigrationFileChange>();
        var directoryChanges = new List<MigrationDirectoryChange>();
        string? currentOldPath = null;
        string? currentNewPath = null;
        string? currentCopyFromPath = null;
        string? currentCopyToPath = null;
        string? currentOldMode = null;
        string? currentNewMode = null;
        var currentHunks = new List<MigrationTextHunk>();
        var removedText = new StringBuilder();
        var addedText = new StringBuilder();
        var originalStartLine = 0;
        var modifiedStartLine = 0;

        void FlushCurrentFileChange()
        {
            if (currentOldPath is null && currentNewPath is null)
                return;

            FlushCurrentHunk();
            var effectiveOldPath = currentCopyFromPath ?? currentOldPath;
            var effectiveNewPath = currentCopyToPath ?? currentNewPath;
            var normalizedBefore = ApplyPathRewrites(NormalizePatchPath(effectiveOldPath));
            var normalizedAfter = ApplyPathRewrites(NormalizePatchPath(effectiveNewPath));
            var kind = DetermineFileChangeKind(effectiveOldPath, effectiveNewPath, currentCopyFromPath, currentCopyToPath, currentOldMode, currentNewMode, currentHunks.Count > 0);
            var textChange = currentHunks.Count == 0
                ? null
                : new MigrationTextChange
                {
                    LineEnding = "\n",
                    Hunks = currentHunks.ToArray()
                };
            var metadata = new Dictionary<string, string>
            {
                [StructuredChangeMetadataKeys.Origin] = Path.GetFileName(patchFilePath)
            };
            if (!string.IsNullOrWhiteSpace(currentOldMode))
                metadata[StructuredChangeMetadataKeys.PatchOldMode] = currentOldMode;
            if (!string.IsNullOrWhiteSpace(currentNewMode))
                metadata[StructuredChangeMetadataKeys.PatchNewMode] = currentNewMode;
            if (!string.IsNullOrWhiteSpace(currentCopyFromPath))
                metadata[StructuredChangeMetadataKeys.PatchCopyFrom] = NormalizePatchPath(currentCopyFromPath);
            if (!string.IsNullOrWhiteSpace(currentCopyToPath))
                metadata[StructuredChangeMetadataKeys.PatchCopyTo] = NormalizePatchPath(currentCopyToPath);

            fileChanges.Add(new MigrationFileChange
            {
                Kind = kind,
                PathBefore = normalizedBefore,
                PathAfter = normalizedAfter,
                TextChange = textChange,
                AppliedPathRewrites = _pathRewrites,
                Metadata = metadata
            });

            TryAddDirectoryChange(directoryChanges, kind, normalizedBefore, normalizedAfter);
            currentOldPath = null;
            currentNewPath = null;
            currentCopyFromPath = null;
            currentCopyToPath = null;
            currentOldMode = null;
            currentNewMode = null;
            currentHunks = new List<MigrationTextHunk>();
        }

        void FlushCurrentHunk()
        {
            if (removedText.Length == 0 && addedText.Length == 0 && originalStartLine == 0 && modifiedStartLine == 0)
                return;

            currentHunks.Add(new MigrationTextHunk
            {
                OriginalStartLine = originalStartLine,
                ModifiedStartLine = modifiedStartLine,
                RemovedText = removedText.ToString(),
                AddedText = addedText.ToString()
            });
            removedText.Clear();
            addedText.Clear();
            originalStartLine = 0;
            modifiedStartLine = 0;
        }

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.StartsWith("--- ", StringComparison.Ordinal))
            {
                FlushCurrentFileChange();
                currentOldPath = line[4..].Trim();
                continue;
            }

            if (line.StartsWith("+++ ", StringComparison.Ordinal))
            {
                currentNewPath = line[4..].Trim();
                continue;
            }

            if (line.StartsWith("copy from ", StringComparison.Ordinal))
            {
                currentCopyFromPath = line[10..].Trim();
                continue;
            }

            if (line.StartsWith("copy to ", StringComparison.Ordinal))
            {
                currentCopyToPath = line[8..].Trim();
                continue;
            }

            if (line.StartsWith("rename from ", StringComparison.Ordinal))
            {
                currentOldPath = line[12..].Trim();
                continue;
            }

            if (line.StartsWith("rename to ", StringComparison.Ordinal))
            {
                currentNewPath = line[10..].Trim();
                continue;
            }

            if (line.StartsWith("old mode ", StringComparison.Ordinal))
            {
                currentOldMode = line[9..].Trim();
                continue;
            }

            if (line.StartsWith("new mode ", StringComparison.Ordinal))
            {
                currentNewMode = line[9..].Trim();
                continue;
            }

            if (line.StartsWith("new file mode ", StringComparison.Ordinal))
            {
                currentOldMode = "000000";
                currentNewMode = line[14..].Trim();
                continue;
            }

            if (line.StartsWith("deleted file mode ", StringComparison.Ordinal))
            {
                currentOldMode = line[18..].Trim();
                currentNewMode = "000000";
                continue;
            }

            if (line.StartsWith("@@ ", StringComparison.Ordinal))
            {
                FlushCurrentHunk();
                ParseHunkHeader(line, out originalStartLine, out modifiedStartLine);
                continue;
            }

            if (line.StartsWith("-", StringComparison.Ordinal) && !line.StartsWith("--- ", StringComparison.Ordinal))
            {
                removedText.AppendLine(line.Length > 1 ? line.Substring(1) : string.Empty);
                continue;
            }

            if (line.StartsWith("+", StringComparison.Ordinal) && !line.StartsWith("+++ ", StringComparison.Ordinal))
            {
                addedText.AppendLine(line.Length > 1 ? line.Substring(1) : string.Empty);
                continue;
            }

            if (line.StartsWith(" ", StringComparison.Ordinal))
                continue;
        }

        FlushCurrentFileChange();
        return new MigrationChangeSet
        {
            ChangeSetId = Path.GetFileNameWithoutExtension(patchFilePath),
            Message = Path.GetFileName(patchFilePath),
            AuthorName = "Patch import",
            Timestamp = new DateTimeOffset(File.GetLastWriteTimeUtc(patchFilePath), TimeSpan.Zero),
            DirectoryChanges = directoryChanges,
            FileChanges = fileChanges,
            Metadata = new Dictionary<string, string>
            {
                [StructuredChangeMetadataKeys.SourceKind] = nameof(MigrationSourceKind.PatchCollection),
                [StructuredChangeMetadataKeys.Origin] = patchFilePath
            }
        };
    }

    private static void ParseHunkHeader(string line, out int originalStartLine, out int modifiedStartLine)
    {
        originalStartLine = 0;
        modifiedStartLine = 0;
        var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
            return;

        originalStartLine = ParseRangeStart(parts[1]);
        modifiedStartLine = ParseRangeStart(parts[2]);
    }

    private static int ParseRangeStart(string token)
    {
        var trimmed = token.TrimStart(['-', '+']);
        var commaIndex = trimmed.IndexOf(",", StringComparison.Ordinal);
        if (commaIndex >= 0)
            trimmed = trimmed[..commaIndex];

        return int.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : 0;
    }

    private static MigrationFileChangeKind DetermineFileChangeKind(
        string? currentOldPath,
        string? currentNewPath,
        string? currentCopyFromPath,
        string? currentCopyToPath,
        string? currentOldMode,
        string? currentNewMode,
        bool hasTextChanges)
    {
        var normalizedBefore = NormalizePatchPath(currentOldPath);
        var normalizedAfter = NormalizePatchPath(currentNewPath);
        if (!string.IsNullOrWhiteSpace(currentCopyFromPath) && !string.IsNullOrWhiteSpace(currentCopyToPath))
            return MigrationFileChangeKind.Copy;

        if (string.IsNullOrWhiteSpace(normalizedBefore))
            return MigrationFileChangeKind.Add;

        if (string.IsNullOrWhiteSpace(normalizedAfter))
            return MigrationFileChangeKind.Delete;

        if (!hasTextChanges && !string.Equals(currentOldMode, currentNewMode, StringComparison.Ordinal))
            return MigrationFileChangeKind.ModeChange;

        return string.Equals(normalizedBefore, normalizedAfter, StringComparison.Ordinal)
            ? MigrationFileChangeKind.Modify
            : MigrationFileChangeKind.Rename;
    }

    private static string NormalizePatchPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || string.Equals(path, "/dev/null", StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        var normalized = path.Trim();
        if (normalized.StartsWith("a/", StringComparison.Ordinal) || normalized.StartsWith("b/", StringComparison.Ordinal))
            normalized = normalized[2..];

        return normalized.Replace('\\', '/');
    }

    private string ApplyPathRewrites(string path)
    {
        var normalizedPath = path;
        foreach (var rewrite in _pathRewrites)
        {
            var comparison = rewrite.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var fromPrefix = rewrite.NormalizeDirectorySeparators
                ? rewrite.FromPrefix.Replace('\\', '/')
                : rewrite.FromPrefix;
            var toPrefix = rewrite.NormalizeDirectorySeparators
                ? rewrite.ToPrefix.Replace('\\', '/')
                : rewrite.ToPrefix;
            if (normalizedPath.StartsWith(fromPrefix, comparison))
            {
                normalizedPath = toPrefix + normalizedPath[fromPrefix.Length..];
            }
        }

        return normalizedPath;
    }

    private static void TryAddDirectoryChange(List<MigrationDirectoryChange> directoryChanges, MigrationFileChangeKind kind, string normalizedBefore, string normalizedAfter)
    {
        if (kind != MigrationFileChangeKind.Rename && kind != MigrationFileChangeKind.Copy)
            return;

        var beforeDirectory = GetDirectoryPath(normalizedBefore);
        var afterDirectory = GetDirectoryPath(normalizedAfter);
        if (string.IsNullOrWhiteSpace(beforeDirectory) || string.IsNullOrWhiteSpace(afterDirectory) || string.Equals(beforeDirectory, afterDirectory, StringComparison.Ordinal))
            return;

        if (directoryChanges.Any(existing => existing.PathBefore == beforeDirectory && existing.PathAfter == afterDirectory && existing.Kind == MapDirectoryKind(kind)))
            return;

        directoryChanges.Add(new MigrationDirectoryChange
        {
            Kind = MapDirectoryKind(kind),
            PathBefore = beforeDirectory,
            PathAfter = afterDirectory,
            Metadata = new Dictionary<string, string>
            {
                [StructuredChangeMetadataKeys.Origin] = nameof(MigrationFileChangeKind)
            }
        });
    }

    private static MigrationDirectoryChangeKind MapDirectoryKind(MigrationFileChangeKind kind)
        => kind == MigrationFileChangeKind.Copy
            ? MigrationDirectoryChangeKind.Copy
            : MigrationDirectoryChangeKind.Rename;

    private static string GetDirectoryPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        var directory = Path.GetDirectoryName(path.Replace('/', Path.DirectorySeparatorChar));
        return string.IsNullOrWhiteSpace(directory)
            ? string.Empty
            : directory.Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');
    }
}
