using NSubstitute;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Archive.Abstractions;
using RepoMigrator.Providers.Archive.Services;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class ArchiveImportPlannerTests
{
    [TestMethod]
    public async Task PrepareAsync_WhenSourceItemsAreAvailable_CreatesDeterministicReadyPlan()
    {
        var source = new ArchiveMigrationSourceDefinition
        {
            LocationKind = ArchiveSourceLocationKind.LocalDirectory,
            Location = @"C:\archives",
            AllowedExtensions = [".zip"]
        }.ToMigrationSourceDefinition();
        var destination = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = @"C:\target", BranchOrTrunk = "main" }
        };
        var sourceProviderFactory = Substitute.For<IMigrationSourceProviderFactory>();
        var sourceProvider = Substitute.For<IMigrationSourceProvider>();
        sourceProviderFactory.Create(source).Returns(sourceProvider);
        sourceProvider.PrepareAsync(source, Arg.Any<CancellationToken>()).Returns(new MigrationSourcePlan
        {
            Source = source,
            Items =
            [
                new MigrationSourcePlanItem { ItemId = "release-2.0.zip", SnapshotId = "release-2.0", SourceIdentifier = CreateTempArchivePath("release-2.0.zip"), DisplayName = "release-2.0.zip" },
                new MigrationSourcePlanItem { ItemId = "release-1.0.zip", SnapshotId = "release-1.0", SourceIdentifier = CreateTempArchivePath("release-1.0.zip"), DisplayName = "release-1.0.zip" }
            ]
        });
        var driverRegistry = Substitute.For<IArchiveDriverRegistry>();
        var driver = Substitute.For<IArchiveDriver>();
        driver.Id.Returns("zip");
        driverRegistry.Resolve(Arg.Any<string>()).Returns(driver);
        driver.InspectAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(callInfo =>
        {
            var archivePath = callInfo.ArgAt<string>(0);
            var fileName = Path.GetFileNameWithoutExtension(archivePath);
            var timestamp = fileName.Contains("1.0", StringComparison.Ordinal)
                ? new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
                : new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero);
            return new ArchiveInspectionResult
            {
                ArchiveFilePath = archivePath,
                DriverId = "zip",
                NewestEntryTimestamp = timestamp,
                Entries =
                [
                    new ArchiveEntryMetadata { EntryPath = fileName.Contains("1.0", StringComparison.Ordinal) ? "product-1.0/src/readme.txt" : "nested/product-2.0/src/readme.txt", IsDirectory = false, Timestamp = timestamp }
                ]
            };
        });
        var planner = new ArchiveImportPlanner(sourceProviderFactory, driverRegistry, new ArchiveOrderingService(), new ArchiveRefNamingService());

        var plan = await planner.PrepareAsync(source, destination, CancellationToken.None);

        Assert.AreEqual(ArchiveImportPlanStatus.Ready, plan.Status);
        Assert.AreEqual(2, plan.Items.Count);
        Assert.AreEqual("release-1.0", plan.Items[0].SnapshotId);
        Assert.AreEqual("release-1.0", plan.Items[0].FinalTagName);
        Assert.AreEqual("releases/release-1.0", plan.Items[0].FinalBranchName);
        Assert.AreEqual("zip", plan.Items[0].ExtensionData[ArchiveImportPlanItemExtensionKeys.DriverId]);
        Assert.AreEqual("product-1.0/src", plan.Items[0].ExtensionData[ArchiveImportPlanItemExtensionKeys.ExtractionRootPath]);
        Assert.AreEqual("2024-01-01T00:00:00.0000000+00:00", plan.Items[0].ExtensionData[ArchiveImportPlanItemExtensionKeys.CommitTimestamp]);
        Assert.AreEqual(nameof(ArchiveSnapshotDescriptor.NewestEntryTimestamp), plan.Items[0].ExtensionData[ArchiveImportPlanItemExtensionKeys.CommitTimestampSource]);
        Assert.IsTrue(plan.SourceProviderData.Count > 0);
    }

    [TestMethod]
    public async Task PrepareAsync_WhenArchiveEntryTimestampIsMissing_UsesArchiveFileLastWriteTimeForCommitTimestamp()
    {
        var source = new ArchiveMigrationSourceDefinition
        {
            LocationKind = ArchiveSourceLocationKind.LocalDirectory,
            Location = @"C:\archives",
            AllowedExtensions = [".zip"]
        }.ToMigrationSourceDefinition();
        var destination = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = @"C:\target", BranchOrTrunk = "main" }
        };
        var sourceProviderFactory = Substitute.For<IMigrationSourceProviderFactory>();
        var sourceProvider = Substitute.For<IMigrationSourceProvider>();
        var archivePath = CreateTempArchivePath("release-1.0.zip");
        var expectedLastWriteTime = new DateTimeOffset(2023, 12, 24, 15, 30, 0, TimeSpan.Zero);

        try
        {
            File.SetLastWriteTimeUtc(archivePath, expectedLastWriteTime.UtcDateTime);
            sourceProviderFactory.Create(source).Returns(sourceProvider);
            sourceProvider.PrepareAsync(source, Arg.Any<CancellationToken>()).Returns(new MigrationSourcePlan
            {
                Source = source,
                Items =
                [
                    new MigrationSourcePlanItem { ItemId = "release-1.0.zip", SnapshotId = "release-1.0", SourceIdentifier = archivePath, DisplayName = "release-1.0.zip" }
                ]
            });
            var driverRegistry = Substitute.For<IArchiveDriverRegistry>();
            var driver = Substitute.For<IArchiveDriver>();
            driver.Id.Returns("zip");
            driverRegistry.Resolve(Arg.Any<string>()).Returns(driver);
            driver.InspectAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new ArchiveInspectionResult
            {
                ArchiveFilePath = archivePath,
                DriverId = "zip",
                NewestEntryTimestamp = null,
                Entries = [new ArchiveEntryMetadata { EntryPath = "release-1.0/readme.txt", IsDirectory = false }]
            });
            var planner = new ArchiveImportPlanner(sourceProviderFactory, driverRegistry, new ArchiveOrderingService(), new ArchiveRefNamingService());

            var plan = await planner.PrepareAsync(source, destination, CancellationToken.None);

            Assert.AreEqual(expectedLastWriteTime.ToString("O"), plan.Items[0].ExtensionData[ArchiveImportPlanItemExtensionKeys.CommitTimestamp]);
            Assert.AreEqual(nameof(ArchiveSnapshotDescriptor.ExternalLastWriteTimestamp), plan.Items[0].ExtensionData[ArchiveImportPlanItemExtensionKeys.CommitTimestampSource]);
        }
        finally
        {
            if (File.Exists(archivePath))
                File.Delete(archivePath);
        }
    }

    [TestMethod]
    public async Task PrepareAsync_WhenArchiveContainsMultipleFiles_PrefersFileNearestToArchiveRoot()
    {
        var source = new ArchiveMigrationSourceDefinition
        {
            LocationKind = ArchiveSourceLocationKind.LocalDirectory,
            Location = @"C:\archives",
            AllowedExtensions = [".zip"]
        }.ToMigrationSourceDefinition();
        var destination = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = @"C:\target", BranchOrTrunk = "main" }
        };
        var sourceProviderFactory = Substitute.For<IMigrationSourceProviderFactory>();
        var sourceProvider = Substitute.For<IMigrationSourceProvider>();
        sourceProviderFactory.Create(source).Returns(sourceProvider);
        sourceProvider.PrepareAsync(source, Arg.Any<CancellationToken>()).Returns(new MigrationSourcePlan
        {
            Source = source,
            Items =
            [
                new MigrationSourcePlanItem { ItemId = "release-1.0.zip", SnapshotId = "release-1.0", SourceIdentifier = CreateTempArchivePath("release-1.0.zip"), DisplayName = "release-1.0.zip" }
            ]
        });
        var driverRegistry = Substitute.For<IArchiveDriverRegistry>();
        var driver = Substitute.For<IArchiveDriver>();
        driver.Id.Returns("zip");
        driverRegistry.Resolve(Arg.Any<string>()).Returns(driver);
        driver.InspectAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new ArchiveInspectionResult
        {
            ArchiveFilePath = "release-1.0.zip",
            DriverId = "zip",
            NewestEntryTimestamp = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Entries =
            [
                new ArchiveEntryMetadata { EntryPath = "release-1.0/deep/src/readme.txt", IsDirectory = false },
                new ArchiveEntryMetadata { EntryPath = "release-1.0/manifest.txt", IsDirectory = false },
                new ArchiveEntryMetadata { EntryPath = "release-1.0/src/app.cs", IsDirectory = false }
            ]
        });
        var planner = new ArchiveImportPlanner(sourceProviderFactory, driverRegistry, new ArchiveOrderingService(), new ArchiveRefNamingService());

        var plan = await planner.PrepareAsync(source, destination, CancellationToken.None);

        Assert.AreEqual("release-1.0", plan.Items[0].ExtensionData[ArchiveImportPlanItemExtensionKeys.ExtractionRootPath]);
    }

    [TestMethod]
    public async Task PrepareAsync_WhenManualExtractionRootOverrideExists_PrefersConfiguredRootPath()
    {
        var sourceDirectoryPath = CreateTempDirectory();
        try
        {
            var configPath = Path.Combine(sourceDirectoryPath, "RepoMigrator.archive-roots.json");
            await File.WriteAllTextAsync(configPath, """
{
  "schemaVersion": 1,
  "entries": [
    {
      "archiveItemId": "release-1.0.zip",
      "rootPath": "manual-root/src"
    }
  ]
}
""");
            var source = new ArchiveMigrationSourceDefinition
            {
                LocationKind = ArchiveSourceLocationKind.LocalDirectory,
                Location = sourceDirectoryPath,
                AllowedExtensions = [".zip"]
            }.ToMigrationSourceDefinition();
            var destination = new MigrationDestinationDefinition
            {
                Kind = MigrationDestinationKind.Repository,
                Repository = new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = @"C:\target", BranchOrTrunk = "main" }
            };
            var sourceProviderFactory = Substitute.For<IMigrationSourceProviderFactory>();
            var sourceProvider = Substitute.For<IMigrationSourceProvider>();
            sourceProviderFactory.Create(source).Returns(sourceProvider);
            sourceProvider.PrepareAsync(source, Arg.Any<CancellationToken>()).Returns(new MigrationSourcePlan
            {
                Source = source,
                Items =
                [
                    new MigrationSourcePlanItem { ItemId = "release-1.0.zip", SnapshotId = "release-1.0", SourceIdentifier = CreateTempArchivePath("release-1.0.zip"), DisplayName = "release-1.0.zip" }
                ]
            });
            var driverRegistry = Substitute.For<IArchiveDriverRegistry>();
            var driver = Substitute.For<IArchiveDriver>();
            driver.Id.Returns("zip");
            driverRegistry.Resolve(Arg.Any<string>()).Returns(driver);
            driver.InspectAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new ArchiveInspectionResult
            {
                ArchiveFilePath = "release-1.0.zip",
                DriverId = "zip",
                NewestEntryTimestamp = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Entries = [new ArchiveEntryMetadata { EntryPath = "auto-root/src/readme.txt", IsDirectory = false }]
            });
            var planner = new ArchiveImportPlanner(sourceProviderFactory, driverRegistry, new ArchiveOrderingService(), new ArchiveRefNamingService());

            var plan = await planner.PrepareAsync(source, destination, CancellationToken.None);

            Assert.AreEqual("manual-root/src", plan.Items[0].ExtensionData[ArchiveImportPlanItemExtensionKeys.ExtractionRootPath]);
        }
        finally
        {
            Directory.Delete(sourceDirectoryPath, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectoryPath);
        return tempDirectoryPath;
    }

    private static string CreateTempArchivePath(string fileName)
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directoryPath);
        var archivePath = Path.Combine(directoryPath, fileName);
        File.WriteAllText(archivePath, "placeholder");
        return archivePath;
    }
}
