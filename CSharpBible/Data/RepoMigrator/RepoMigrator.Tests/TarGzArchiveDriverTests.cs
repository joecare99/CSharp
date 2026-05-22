using System.Formats.Tar;
using System.IO.Compression;
using RepoMigrator.Providers.Archive.Services;
using RepoMigrator.Providers.Compression.TarGz;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class TarGzArchiveDriverTests
{
    [TestMethod]
    public async Task InspectAsync_WhenValidTarGzArchive_ReturnsEntriesAndNewestTimestamp()
    {
        var archiveFilePath = CreateTempFilePath(".tar.gz");
        try
        {
            CreateTarGzArchive(archiveFilePath, static writer =>
            {
                var entryOne = new PaxTarEntry(TarEntryType.RegularFile, "alpha.txt")
                {
                    DataStream = CreateEntryStream("alpha"),
                    ModificationTime = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero)
                };
                writer.WriteEntry(entryOne);

                var entryTwo = new PaxTarEntry(TarEntryType.RegularFile, "nested/beta.txt")
                {
                    DataStream = CreateEntryStream("beta"),
                    ModificationTime = new DateTimeOffset(2024, 1, 2, 11, 0, 0, TimeSpan.Zero)
                };
                writer.WriteEntry(entryTwo);
            });

            var driver = new TarGzArchiveDriver();

            var result = await driver.InspectAsync(archiveFilePath, CancellationToken.None);

            Assert.AreEqual("tar.gz", result.DriverId);
            Assert.AreEqual(Path.GetFullPath(archiveFilePath), result.ArchiveFilePath);
            Assert.AreEqual(2, result.Entries.Count);
            Assert.IsNotNull(result.NewestEntryTimestamp);
            Assert.AreEqual(new DateTime(2024, 1, 2, 11, 0, 0), result.NewestEntryTimestamp.Value.DateTime);
            Assert.AreEqual("alpha.txt", result.Entries[0].EntryPath);
        }
        finally
        {
            DeleteFileIfExists(archiveFilePath);
        }
    }

    [TestMethod]
    public async Task ExtractToAsync_WhenValidTarGzArchive_ExtractsFiles()
    {
        var archiveFilePath = CreateTempFilePath(".tar.gz");
        var targetDirectoryPath = CreateTempDirectory();
        try
        {
            CreateTarGzArchive(archiveFilePath, static writer =>
            {
                var entry = new PaxTarEntry(TarEntryType.RegularFile, "nested/gamma.txt")
                {
                    DataStream = CreateEntryStream("gamma"),
                    ModificationTime = new DateTimeOffset(2024, 1, 2, 11, 0, 0, TimeSpan.Zero)
                };
                writer.WriteEntry(entry);
            });

            var driver = new TarGzArchiveDriver();

            await driver.ExtractToAsync(archiveFilePath, targetDirectoryPath, CancellationToken.None);

            var extractedFilePath = Path.Combine(targetDirectoryPath, "nested", "gamma.txt");
            Assert.IsTrue(File.Exists(extractedFilePath));
            Assert.AreEqual("gamma", File.ReadAllText(extractedFilePath));
        }
        finally
        {
            DeleteFileIfExists(archiveFilePath);
            TryDeleteDirectory(targetDirectoryPath);
        }
    }

    [TestMethod]
    public async Task InspectAsync_WhenArchiveIsInvalid_ThrowsInvalidDataException()
    {
        var archiveFilePath = CreateTempFilePath(".tar.gz");
        try
        {
            File.WriteAllText(archiveFilePath, "not a tar.gz");
            var driver = new TarGzArchiveDriver();

            await Assert.ThrowsAsync<InvalidDataException>(() => driver.InspectAsync(archiveFilePath, CancellationToken.None));
        }
        finally
        {
            DeleteFileIfExists(archiveFilePath);
        }
    }

    [TestMethod]
    public void CanHandle_WhenArchiveHasTarGzExtension_ReturnsTrue()
    {
        var driver = new TarGzArchiveDriver();

        Assert.IsTrue(driver.CanHandle("archive.TAR.GZ"));
        Assert.IsTrue(driver.CanHandle("archive.TGZ"));
        Assert.IsFalse(driver.CanHandle("archive.zip"));
    }

    [TestMethod]
    public void Resolve_WhenTarGzDriverRegistered_ReturnsTarGzDriver()
    {
        var tarGzDriver = new TarGzArchiveDriver();
        var registry = new ArchiveDriverRegistry([tarGzDriver]);

        var resolvedDriver = registry.Resolve("sample.tar.gz");

        Assert.AreSame(tarGzDriver, resolvedDriver);
    }

    [TestMethod]
    public void Resolve_WhenTgzArchiveIsUsed_ReturnsTarGzDriver()
    {
        var tarGzDriver = new TarGzArchiveDriver();
        var registry = new ArchiveDriverRegistry([tarGzDriver]);

        var resolvedDriver = registry.Resolve("sample.tgz");

        Assert.AreSame(tarGzDriver, resolvedDriver);
    }

    private static void CreateTarGzArchive(string archiveFilePath, Action<TarWriter> writeEntries)
    {
        using var fileStream = File.Create(archiveFilePath);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress, leaveOpen: false);
        using var tarWriter = new TarWriter(gzipStream, TarEntryFormat.Pax, leaveOpen: false);
        writeEntries(tarWriter);
    }

    private static MemoryStream CreateEntryStream(string content)
        => new(System.Text.Encoding.UTF8.GetBytes(content), writable: false);

    private static string CreateTempFilePath(string extension)
        => Path.Combine(Path.GetTempPath(), $"RepoMigrator-{Guid.NewGuid():N}{extension}");

    private static string CreateTempDirectory()
    {
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectoryPath);
        return tempDirectoryPath;
    }

    private static void DeleteFileIfExists(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    private static void TryDeleteDirectory(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
                return;

            foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
                File.SetAttributes(filePath, FileAttributes.Normal);

            Directory.Delete(directoryPath, recursive: true);
        }
        catch
        {
        }
    }
}
