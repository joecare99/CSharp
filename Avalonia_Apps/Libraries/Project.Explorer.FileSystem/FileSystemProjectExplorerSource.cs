using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Project.Explorer;

namespace Project.Explorer.FileSystem;

/// <summary>Builds a deterministic explorer hierarchy from an accessible local directory tree.</summary>
public sealed class FileSystemProjectExplorerSource : IProjectExplorerSource
{
    private readonly ISet<string>? _supportedExtensions;

    /// <summary>Initializes a source that includes all files or only the supplied extensions.</summary>
    public FileSystemProjectExplorerSource(IEnumerable<string>? supportedExtensions = null)
    {
        _supportedExtensions = supportedExtensions is null
            ? null
            : new HashSet<string>(
                supportedExtensions.Select(NormalizeExtension),
                StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public Task<ProjectExplorerSnapshot> RefreshAsync(string rootPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("An explorer root path is required.", nameof(rootPath));
        }

        cancellationToken.ThrowIfCancellationRequested();
        string normalizedRootPath = Path.GetFullPath(rootPath);
        ProjectExplorerItem rootItem = CreateDirectoryItem(
            normalizedRootPath,
            ProjectExplorerItemKind.Project,
            cancellationToken);
        return Task.FromResult(new ProjectExplorerSnapshot(normalizedRootPath, [rootItem]));
    }

    private ProjectExplorerItem CreateDirectoryItem(
        string path,
        ProjectExplorerItemKind kind,
        CancellationToken cancellationToken)
    {
        string name = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (string.IsNullOrEmpty(name))
        {
            name = path;
        }

        if (!Directory.Exists(path))
        {
            return new ProjectExplorerItem(CreateId(kind, path), name, path, kind, isAvailable: false);
        }

        try
        {
            DirectoryInfo directory = new(path);
            if ((directory.Attributes & FileAttributes.ReparsePoint) != 0 && kind != ProjectExplorerItemKind.Project)
            {
                return new ProjectExplorerItem(CreateId(kind, path), name, path, kind);
            }

            List<ProjectExplorerItem> children = [];
            foreach (string directoryPath in Directory.EnumerateDirectories(path)
                         .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
            {
                cancellationToken.ThrowIfCancellationRequested();
                children.Add(CreateDirectoryItem(directoryPath, ProjectExplorerItemKind.Folder, cancellationToken));
            }

            foreach (string filePath in Directory.EnumerateFiles(path)
                         .Where(IsSupportedFile)
                         .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
            {
                cancellationToken.ThrowIfCancellationRequested();
                children.Add(new ProjectExplorerItem(
                    CreateId(ProjectExplorerItemKind.File, filePath),
                    Path.GetFileName(filePath),
                    filePath,
                    ProjectExplorerItemKind.File));
            }

            return new ProjectExplorerItem(CreateId(kind, path), name, path, kind, children: children);
        }
        catch (UnauthorizedAccessException)
        {
            return new ProjectExplorerItem(CreateId(kind, path), name, path, kind, isAvailable: false);
        }
        catch (IOException)
        {
            return new ProjectExplorerItem(CreateId(kind, path), name, path, kind, isAvailable: false);
        }
    }

    private bool IsSupportedFile(string path) =>
        _supportedExtensions is null || _supportedExtensions.Contains(Path.GetExtension(path));

    private static string CreateId(ProjectExplorerItemKind kind, string path) =>
        $"{kind}:{Path.GetFullPath(path)}";

    private static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentException("A supported extension is required.", nameof(extension));
        }

        return extension[0] == '.' ? extension : $".{extension}";
    }
}
