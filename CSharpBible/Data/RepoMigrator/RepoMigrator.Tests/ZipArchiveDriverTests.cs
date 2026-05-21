using RepoMigrator.Providers.Archive.Services;
using RepoMigrator.Providers.Compression.Zip;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class ZipArchiveDriverTests
{
    [TestMethod]
    public async Task InspectAsync_WhenValidZipArchive_ReturnsEntriesAndNewestTimestamp()
    {
        var archiveFilePath = CreateTempFilePath(".zip");
        try
        {
            using (var archive = System.IO.Compression.ZipFile.Open(archiveFilePath, System.IO.Compression.ZipArchiveMode.Create))
            {
                var entryOne = archive.CreateEntry("alpha.txt");
                entryOne.LastWriteTime = new DateTimeOffset(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
                using (var writer = new StreamWriter(entryOne.Open()))
                    writer.Write("alpha");

                var entryTwo = archive.CreateEntry("nested/beta.txt");
                entryTwo.LastWriteTime = new DateTimeOffset(2024, 1, 2, 11, 0, 0, TimeSpan.Zero);
                using (var writer = new StreamWriter(entryTwo.Open()))
                    writer.Write("beta");
            }

            var driver = new ZipArchiveDriver();

            var result = await driver.InspectAsync(archiveFilePath, CancellationToken.None);

            Assert.AreEqual("zip", result.DriverId);
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
    public async Task ExtractToAsync_WhenValidZipArchive_ExtractsFiles()
    {
        var archiveFilePath = CreateTempFilePath(".zip");
        var targetDirectoryPath = CreateTempDirectory();
        try
        {
            using (var archive = System.IO.Compression.ZipFile.Open(archiveFilePath, System.IO.Compression.ZipArchiveMode.Create))
            {
                var entry = archive.CreateEntry("nested/gamma.txt");
                using var writer = new StreamWriter(entry.Open());
                writer.Write("gamma");
            }

            var driver = new ZipArchiveDriver();

            await driver.ExtractToAsync(archiveFilePath, targetDirectoryPath, CancellationToken.None);

            var extractedFilePath = Path.Combine(targetDirectoryPath, "nested", "gamma.txt");
            Assert.IsTrue(File.Exists(extractedFilePath));
            Assert.AreEqual("gamma", File.ReadAllText(extractedFilePath));
        }
        finally
        {
            DeleteFileIfExists(archiveFilePath);
            Directory.Delete(targetDirectoryPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task InspectAsync_WhenArchiveIsInvalid_ThrowsInvalidDataException()
    {
        var archiveFilePath = CreateTempFilePath(".zip");
        try
        {
            File.WriteAllText(archiveFilePath, "not a zip");
            var driver = new ZipArchiveDriver();

            await Assert.ThrowsAsync<InvalidDataException>(() => driver.InspectAsync(archiveFilePath, CancellationToken.None));
        }
        finally
        {
            DeleteFileIfExists(archiveFilePath);
        }
    }

    [TestMethod]
    public void CanHandle_WhenArchiveHasZipExtension_ReturnsTrue()
    {
        var driver = new ZipArchiveDriver();

        Assert.IsTrue(driver.CanHandle("archive.ZIP"));
        Assert.IsFalse(driver.CanHandle("archive.tar.gz"));
    }

    [TestMethod]
    public void Resolve_WhenZipDriverRegistered_ReturnsZipDriver()
    {
        var zipDriver = new ZipArchiveDriver();
        var registry = new ArchiveDriverRegistry([zipDriver]);

        var resolvedDriver = registry.Resolve("sample.zip");

        Assert.AreSame(zipDriver, resolvedDriver);
    }

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
}
