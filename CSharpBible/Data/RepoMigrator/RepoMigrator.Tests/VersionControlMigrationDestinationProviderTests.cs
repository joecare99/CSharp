using LibGit2Sharp;
using NSubstitute;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Git;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class VersionControlMigrationDestinationProviderTests
{
    [TestMethod]
    public async Task InitializeAndWriteSnapshotAsync_WhenGitRepositoryDestinationIsUsed_DelegatesToVersionControlProvider()
    {
        var versionControlProvider = Substitute.For<IVersionControlProvider, IGitTargetRefOperations>();
        var destinationProvider = new VersionControlMigrationDestinationProvider(versionControlProvider);
        var destination = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = @"C:\target", BranchOrTrunk = "main" }
        };
        var progress = Substitute.For<IMigrationProgress>();
        var workDir = CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(workDir, "README.md"), "hello");

            await destinationProvider.InitializeAsync(destination, CancellationToken.None);
            await destinationProvider.WriteSnapshotAsync(workDir, new MigrationDestinationCommit
            {
                SnapshotId = "snapshot-1",
                Message = "import snapshot",
                AuthorName = "alice",
                AuthorEmail = "alice@example.org",
                Timestamp = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero),
                DestinationReference = "releases/1.0"
            }, progress, CancellationToken.None);
            await destinationProvider.FinalizeAsync(CancellationToken.None);

            await versionControlProvider.Received(1).InitializeTargetAsync(destination.Repository!, true, CancellationToken.None);
            await versionControlProvider.Received(1).CommitSnapshotAsync(
                workDir,
                Arg.Is<CommitMetadata>(metadata => metadata.Message == "import snapshot"
                    && metadata.AuthorName == "alice"
                    && metadata.AuthorEmail == "alice@example.org"
                    && metadata.TargetBranch == "releases/1.0"),
                progress,
                CancellationToken.None);
            await versionControlProvider.Received(1).FlushAsync(CancellationToken.None);
        }
        finally
        {
            Directory.Delete(workDir, recursive: true);
        }
    }

    [TestMethod]
    public async Task EnsureTagAndBranchAsync_WhenProviderSupportsGitRefOperations_DelegatesToUnderlyingProvider()
    {
        var versionControlProvider = Substitute.For<IVersionControlProvider, IGitTargetRefOperations>();
        var gitOperations = (IGitTargetRefOperations)versionControlProvider;
        var destinationProvider = new VersionControlMigrationDestinationProvider(versionControlProvider);
        var destination = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = @"C:\target", BranchOrTrunk = "main" }
        };

        await destinationProvider.InitializeAsync(destination, CancellationToken.None);
        await destinationProvider.EnsureTagAsync("v1.0", "abc123", CancellationToken.None);
        await destinationProvider.EnsureBranchAsync("releases/1.0", "abc123", CancellationToken.None);

        await gitOperations.Received(1).EnsureTagAsync("v1.0", "abc123", CancellationToken.None);
        await gitOperations.Received(1).EnsureBranchAsync("releases/1.0", "abc123", CancellationToken.None);
    }

    private static string CreateTempDirectory()
    {
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectoryPath);
        return tempDirectoryPath;
    }
}
