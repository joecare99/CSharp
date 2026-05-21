using RepoMigrator.Core;
using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Archive.Services;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class DirectoryArchiveSnapshotSourceProviderTests
{
    [TestMethod]
    public async Task PrepareAsync_WhenAbsoluteDirectoryPathIsUsed_ReturnsDeterministicallyOrderedItems()
    {
        var tempDirectoryPath = CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(tempDirectoryPath, "b.zip"), "b");
            File.WriteAllText(Path.Combine(tempDirectoryPath, "A.zip"), "a");
            var provider = new DirectoryArchiveSnapshotSourceProvider();
            var source = new MigrationSourceDefinition
            {
                Kind = MigrationSourceKind.ArchiveCollection,
                ProviderData = new ArchiveMigrationSourceDefinition
                {
                    LocationKind = ArchiveSourceLocationKind.LocalDirectory,
                    Location = tempDirectoryPath,
                    AllowedExtensions = [".zip"]
                }.ToMigrationSourceDefinition().ProviderData
            };

            var plan = await provider.PrepareAsync(source, CancellationToken.None);

            Assert.AreEqual(2, plan.Items.Count);
            CollectionAssert.AreEqual(new[] { "A.zip", "b.zip" }, plan.Items.Select(item => item.ItemId).ToArray());
        }
        finally
        {
            Directory.Delete(tempDirectoryPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task PrepareAsync_WhenDefaultExtensionsAreUsed_IncludesTgzArchives()
    {
        var tempDirectoryPath = CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(tempDirectoryPath, "release-1.0.tar.gz"), "tar.gz");
            File.WriteAllText(Path.Combine(tempDirectoryPath, "release-2.0.tgz"), "tgz");
            var provider = new DirectoryArchiveSnapshotSourceProvider();
            var source = new MigrationSourceDefinition
            {
                Kind = MigrationSourceKind.ArchiveCollection,
                ProviderData = new ArchiveMigrationSourceDefinition
                {
                    LocationKind = ArchiveSourceLocationKind.LocalDirectory,
                    Location = tempDirectoryPath
                }.ToMigrationSourceDefinition().ProviderData
            };

            var plan = await provider.PrepareAsync(source, CancellationToken.None);

            Assert.AreEqual(2, plan.Items.Count);
            CollectionAssert.AreEquivalent(new[] { "release-1.0.tar.gz", "release-2.0.tgz" }, plan.Items.Select(item => item.ItemId).ToArray());
        }
        finally
        {
            Directory.Delete(tempDirectoryPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task PrepareAsync_WhenRelativeDirectoryPathIsUsed_ResolvesFromWorkspaceRoot()
    {
        var workspaceRootPath = CreateTempDirectory();
        try
        {
            var archivesPath = Path.Combine(workspaceRootPath, "archives");
            Directory.CreateDirectory(archivesPath);
            File.WriteAllText(Path.Combine(archivesPath, "release.zip"), "x");
            var provider = new DirectoryArchiveSnapshotSourceProvider(workspaceRootPath);
            var source = new MigrationSourceDefinition
            {
                Kind = MigrationSourceKind.ArchiveCollection,
                ProviderData = new ArchiveMigrationSourceDefinition
                {
                    LocationKind = ArchiveSourceLocationKind.LocalDirectory,
                    Location = ".\\archives",
                    AllowedExtensions = [".zip"]
                }.ToMigrationSourceDefinition().ProviderData
            };

            var plan = await provider.PrepareAsync(source, CancellationToken.None);

            Assert.AreEqual(1, plan.Items.Count);
            Assert.AreEqual("release.zip", plan.Items[0].ItemId);
            Assert.AreEqual(Path.GetFullPath(Path.Combine(archivesPath, "release.zip")), plan.Items[0].SourceIdentifier);
        }
        finally
        {
            Directory.Delete(workspaceRootPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task PrepareAsync_WhenRecursiveScanDisabled_ExcludesNestedFiles()
    {
        var tempDirectoryPath = CreateTempDirectory();
        try
        {
            Directory.CreateDirectory(Path.Combine(tempDirectoryPath, "nested"));
            File.WriteAllText(Path.Combine(tempDirectoryPath, "root.zip"), "root");
            File.WriteAllText(Path.Combine(tempDirectoryPath, "nested", "child.zip"), "child");
            var provider = new DirectoryArchiveSnapshotSourceProvider();
            var source = new MigrationSourceDefinition
            {
                Kind = MigrationSourceKind.ArchiveCollection,
                ProviderData = new ArchiveMigrationSourceDefinition
                {
                    LocationKind = ArchiveSourceLocationKind.LocalDirectory,
                    Location = tempDirectoryPath,
                    AllowedExtensions = [".zip"],
                    RecursiveDirectoryScan = false
                }.ToMigrationSourceDefinition().ProviderData
            };

            var plan = await provider.PrepareAsync(source, CancellationToken.None);

            Assert.AreEqual(1, plan.Items.Count);
            Assert.AreEqual("root.zip", plan.Items[0].ItemId);
        }
        finally
        {
            Directory.Delete(tempDirectoryPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task PrepareAsync_WhenNoMatchingArchivesExist_ThrowsInvalidOperationException()
    {
        var tempDirectoryPath = CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(tempDirectoryPath, "notes.txt"), "notes");
            var provider = new DirectoryArchiveSnapshotSourceProvider();
            var source = new MigrationSourceDefinition
            {
                Kind = MigrationSourceKind.ArchiveCollection,
                ProviderData = new ArchiveMigrationSourceDefinition
                {
                    LocationKind = ArchiveSourceLocationKind.LocalDirectory,
                    Location = tempDirectoryPath,
                    AllowedExtensions = [".zip"]
                }.ToMigrationSourceDefinition().ProviderData
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => provider.PrepareAsync(source, CancellationToken.None));
        }
        finally
        {
            Directory.Delete(tempDirectoryPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task PrepareAsync_WhenDirectoryDoesNotExist_ThrowsDirectoryNotFoundException()
    {
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-{Guid.NewGuid():N}");
        var provider = new DirectoryArchiveSnapshotSourceProvider();
        var source = new MigrationSourceDefinition
        {
            Kind = MigrationSourceKind.ArchiveCollection,
            ProviderData = new ArchiveMigrationSourceDefinition
            {
                LocationKind = ArchiveSourceLocationKind.LocalDirectory,
                Location = tempDirectoryPath,
                AllowedExtensions = [".zip"]
            }.ToMigrationSourceDefinition().ProviderData
        };

        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => provider.PrepareAsync(source, CancellationToken.None));
    }

    [TestMethod]
    public void CanHandle_WhenSourceIsLocalArchiveDirectory_ReturnsTrue()
    {
        var provider = new DirectoryArchiveSnapshotSourceProvider();
        var source = new MigrationSourceDefinition
        {
            Kind = MigrationSourceKind.ArchiveCollection,
            ProviderData = new ArchiveMigrationSourceDefinition
            {
                LocationKind = ArchiveSourceLocationKind.LocalDirectory,
                Location = @"C:\archives"
            }.ToMigrationSourceDefinition().ProviderData
        };

        Assert.IsTrue(provider.CanHandle(source));
    }

    private static string CreateTempDirectory()
    {
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectoryPath);
        return tempDirectoryPath;
    }
}
