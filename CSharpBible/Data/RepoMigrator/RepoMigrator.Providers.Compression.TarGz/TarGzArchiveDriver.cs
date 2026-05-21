using System.Formats.Tar;
using System.IO.Compression;
using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Archive.Abstractions;

namespace RepoMigrator.Providers.Compression.TarGz;

/// <summary>
/// Provides archive inspection and extraction for gzip-compressed tar files.
/// </summary>
public sealed class TarGzArchiveDriver : IArchiveDriver
{
    /// <inheritdoc/>
    public string Id => "tar.gz";

    /// <inheritdoc/>
    public bool CanHandle(string archivePathOrName)
        => !string.IsNullOrWhiteSpace(archivePathOrName)
            && (archivePathOrName.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase)
                || archivePathOrName.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase));

    /// <inheritdoc/>
    public Task<ArchiveInspectionResult> InspectAsync(string archiveFilePath, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(archiveFilePath);
        EnsureArchiveExists(archiveFilePath);

        using var fileStream = File.OpenRead(archiveFilePath);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress, leaveOpen: false);
        using var tarReader = new TarReader(gzipStream, leaveOpen: false);
        var entries = new List<ArchiveEntryMetadata>();
        TarEntry? entry;
        while ((entry = tarReader.GetNextEntry()) is not null)
        {
            ct.ThrowIfCancellationRequested();
            entries.Add(new ArchiveEntryMetadata
            {
                EntryPath = entry.Name,
                IsDirectory = entry.EntryType is TarEntryType.Directory,
                Size = entry.EntryType is TarEntryType.Directory ? null : entry.Length,
                Timestamp = entry.ModificationTime
            });
        }

        return Task.FromResult(new ArchiveInspectionResult
        {
            ArchiveFilePath = Path.GetFullPath(archiveFilePath),
            DriverId = Id,
            NewestEntryTimestamp = entries.Where(entry => entry.Timestamp.HasValue).Select(entry => entry.Timestamp).Max(),
            Entries = entries
        });
    }

    /// <inheritdoc/>
    public Task ExtractToAsync(string archiveFilePath, string targetDirectory, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(archiveFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetDirectory);
        EnsureArchiveExists(archiveFilePath);
        Directory.CreateDirectory(targetDirectory);

        using var fileStream = File.OpenRead(archiveFilePath);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress, leaveOpen: false);
        TarFile.ExtractToDirectory(gzipStream, targetDirectory, overwriteFiles: true);
        return Task.CompletedTask;
    }

    private static void EnsureArchiveExists(string archiveFilePath)
    {
        if (!File.Exists(archiveFilePath))
            throw new FileNotFoundException($"TarGz archive '{archiveFilePath}' does not exist.", archiveFilePath);
    }
}
