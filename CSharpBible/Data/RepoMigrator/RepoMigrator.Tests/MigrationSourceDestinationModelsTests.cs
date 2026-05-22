using RepoMigrator.Core;
using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Patch;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class MigrationSourceDestinationModelsTests
{
    [TestMethod]
    public void ArchiveMigrationSourceDefinition_Defaults_AreInitialized()
    {
        var definition = new ArchiveMigrationSourceDefinition();

        Assert.AreEqual(ArchiveSourceLocationKind.LocalDirectory, definition.LocationKind);
        Assert.AreEqual(string.Empty, definition.Location);
        Assert.AreEqual(0, definition.AllowedExtensions.Count);
        Assert.IsFalse(definition.AllowRelativeLinks);
        Assert.IsFalse(definition.RecursiveDirectoryScan);
    }

    [TestMethod]
    public void SequentialArchiveDestinationDefinition_Defaults_AreInitialized()
    {
        var definition = new SequentialArchiveDestinationDefinition();

        Assert.AreEqual(string.Empty, definition.OutputDirectory);
        Assert.AreEqual(".zip", definition.ArchiveFileExtension);
        Assert.IsFalse(definition.OverwriteExistingArchives);
    }

    [TestMethod]
    public void PatchMigrationSourceDefinition_Defaults_AreInitialized()
    {
        var definition = new PatchMigrationSourceDefinition();

        Assert.AreEqual(PatchSourceLocationKind.LocalDirectory, definition.LocationKind);
        Assert.AreEqual(string.Empty, definition.Location);
        Assert.AreEqual(0, definition.AllowedExtensions.Count);
        Assert.IsFalse(definition.RecursiveDirectoryScan);
        Assert.AreEqual(0, definition.PathRewrites.Count);
    }

    [TestMethod]
    public void MigrationSourceDefinition_Defaults_AreInitialized()
    {
        var definition = new MigrationSourceDefinition();

        Assert.AreEqual(MigrationSourceKind.Repository, definition.Kind);
        Assert.IsNull(definition.Repository);
        Assert.AreEqual(0, definition.ProviderData.Count);
    }

    [TestMethod]
    public void MigrationDestinationDefinition_Defaults_AreInitialized()
    {
        var definition = new MigrationDestinationDefinition();

        Assert.AreEqual(MigrationDestinationKind.Repository, definition.Kind);
        Assert.IsNull(definition.Repository);
        Assert.AreEqual(0, definition.ProviderData.Count);
    }

    [TestMethod]
    public void MigrationDestinationCommit_Defaults_AreInitialized()
    {
        var commit = new MigrationDestinationCommit();

        Assert.AreEqual(string.Empty, commit.SnapshotId);
        Assert.AreEqual(string.Empty, commit.Message);
        Assert.AreEqual(string.Empty, commit.AuthorName);
        Assert.IsNull(commit.AuthorEmail);
        Assert.AreEqual(default, commit.Timestamp);
        Assert.IsNull(commit.DestinationReference);
    }

    [TestMethod]
    public void MigrationSourceDefinition_AssignedValues_ArePreserved()
    {
        var archiveSource = new ArchiveMigrationSourceDefinition
        {
            LocationKind = ArchiveSourceLocationKind.HttpIndex,
            Location = "https://example.invalid/releases/",
            AllowedExtensions = [".zip", ".tar.gz"],
            AllowRelativeLinks = true,
            RecursiveDirectoryScan = true
        };
        var repository = new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = @"C:\source", BranchOrTrunk = "main" };
        var definition = new MigrationSourceDefinition
        {
            Kind = MigrationSourceKind.ArchiveCollection,
            Repository = repository,
            ProviderData = archiveSource.ToMigrationSourceDefinition().ProviderData
        };

        Assert.AreEqual(MigrationSourceKind.ArchiveCollection, definition.Kind);
        Assert.AreSame(repository, definition.Repository);
        var reconstructedArchiveSource = ArchiveMigrationSourceDefinition.FromMigrationSourceDefinition(definition);
        Assert.AreEqual(2, reconstructedArchiveSource.AllowedExtensions.Count);
    }

    [TestMethod]
    public void PatchMigrationSourceDefinition_AssignedValues_ArePreserved()
    {
        var patchSource = new PatchMigrationSourceDefinition
        {
            LocationKind = PatchSourceLocationKind.LocalDirectory,
            Location = @"C:\patches",
            AllowedExtensions = [".patch", ".diff"],
            RecursiveDirectoryScan = true,
            PathRewrites =
            [
                new PathRewriteRule
                {
                    FromPrefix = "old/root",
                    ToPrefix = "new/root",
                    NormalizeDirectorySeparators = true,
                    IgnoreCase = true
                }
            ]
        };
        var definition = patchSource.ToMigrationSourceDefinition();

        Assert.AreEqual(MigrationSourceKind.PatchCollection, definition.Kind);
        var reconstructedPatchSource = PatchMigrationSourceDefinition.FromMigrationSourceDefinition(definition);
        Assert.AreEqual(@"C:\patches", reconstructedPatchSource.Location);
        Assert.AreEqual(2, reconstructedPatchSource.AllowedExtensions.Count);
        Assert.IsTrue(reconstructedPatchSource.RecursiveDirectoryScan);
        Assert.AreEqual(1, reconstructedPatchSource.PathRewrites.Count);
        Assert.AreEqual("new/root", reconstructedPatchSource.PathRewrites[0].ToPrefix);
    }

    [TestMethod]
    public void MigrationDestinationDefinition_AssignedValues_ArePreserved()
    {
        var repository = new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = @"C:\target", BranchOrTrunk = "main" };
        var sequentialArchiveDestination = new SequentialArchiveDestinationDefinition
        {
            OutputDirectory = @"C:\exports",
            ArchiveFileExtension = ".zip",
            OverwriteExistingArchives = true
        };
        var definition = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.SequentialArchiveCollection,
            Repository = repository,
            ProviderData = sequentialArchiveDestination.ToMigrationDestinationDefinition().ProviderData
        };

        Assert.AreEqual(MigrationDestinationKind.SequentialArchiveCollection, definition.Kind);
        Assert.AreSame(repository, definition.Repository);
        var reconstructedDestination = SequentialArchiveDestinationDefinition.FromMigrationDestinationDefinition(definition);
        Assert.IsTrue(reconstructedDestination.OverwriteExistingArchives);
    }
}
