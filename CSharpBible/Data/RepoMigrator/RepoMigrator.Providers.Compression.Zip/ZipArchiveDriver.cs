using System.IO.Compression;
using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Archive.Abstractions;

namespace RepoMigrator.Providers.Compression.Zip;

/// <summary>
/// Provides archive inspection and extraction for Zip files.
/// </summary>
public sealed class ZipArchiveDriver : IArchiveDriver
{
    /// <inheritdoc/>
    public string Id => "zip";

    /// <inheritdoc/>
    public bool CanHandle(string archivePathOrName)
        => !string.IsNullOrWhiteSpace(archivePathOrName)
            && archivePathOrName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public Task<ArchiveInspectionResult> InspectAsync(string archiveFilePath, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(archiveFilePath);
        EnsureArchiveExists(archiveFilePath);

        using var stream = File.OpenRead(archiveFilePath);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
        var entries = archive.Entries
            .Select(entry => new ArchiveEntryMetadata
            {
                EntryPath = entry.FullName,
                IsDirectory = string.IsNullOrEmpty(entry.Name) && entry.FullName.EndsWith("/", StringComparison.Ordinal),
                Size = string.IsNullOrEmpty(entry.Name) && entry.FullName.EndsWith("/", StringComparison.Ordinal)
                    ? null
                    : entry.Length,
                Timestamp = entry.LastWriteTime == default
                    ? null
                    : entry.LastWriteTime
            })
            .ToArray();

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
        ZipFile.ExtractToDirectory(archiveFilePath, targetDirectory, overwriteFiles: true);
        return Task.CompletedTask;
    }

    private static void EnsureArchiveExists(string archiveFilePath)
    {
        if (!File.Exists(archiveFilePath))
            throw new FileNotFoundException($"Zip archive '{archiveFilePath}' does not exist.", archiveFilePath);
    }
}
