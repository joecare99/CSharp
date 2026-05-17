using RepoMigrator.Core;

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
    public void MigrationSourceDefinition_Defaults_AreInitialized()
    {
        var definition = new MigrationSourceDefinition();

        Assert.AreEqual(MigrationSourceKind.Repository, definition.Kind);
        Assert.IsNull(definition.Repository);
        Assert.IsNull(definition.ArchiveSource);
    }

    [TestMethod]
    public void MigrationDestinationDefinition_Defaults_AreInitialized()
    {
        var definition = new MigrationDestinationDefinition();

        Assert.AreEqual(MigrationDestinationKind.Repository, definition.Kind);
        Assert.IsNull(definition.Repository);
        Assert.IsNull(definition.SequentialArchiveDestination);
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
        var repository = new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = @"C:\source", BranchOrTrunk = "main" };
        var definition = new MigrationSourceDefinition
        {
            Kind = MigrationSourceKind.ArchiveCollection,
            Repository = repository,
            ArchiveSource = archiveSource
        };

        Assert.AreEqual(MigrationSourceKind.ArchiveCollection, definition.Kind);
        Assert.AreSame(repository, definition.Repository);
        Assert.AreSame(archiveSource, definition.ArchiveSource);
        Assert.AreEqual(2, definition.ArchiveSource.AllowedExtensions.Count);
    }

    [TestMethod]
    public void MigrationDestinationDefinition_AssignedValues_ArePreserved()
    {
        var repository = new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = @"C:\target", BranchOrTrunk = "main" };
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
            SequentialArchiveDestination = sequentialArchiveDestination
        };

        Assert.AreEqual(MigrationDestinationKind.SequentialArchiveCollection, definition.Kind);
        Assert.AreSame(repository, definition.Repository);
        Assert.AreSame(sequentialArchiveDestination, definition.SequentialArchiveDestination);
        Assert.IsTrue(definition.SequentialArchiveDestination.OverwriteExistingArchives);
    }
}
