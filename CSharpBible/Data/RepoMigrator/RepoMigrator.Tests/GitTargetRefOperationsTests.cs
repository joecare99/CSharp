using LibGit2Sharp;
using RepoMigrator.Core;
using RepoMigrator.Providers.Git;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class GitTargetRefOperationsTests
{
    [TestMethod]
    public async Task EnsureTagAndBranchAsync_WhenCommitExists_CreatesMissingRefsIdempotently()
    {
        var repositoryPath = CreateTempDirectory();
        var snapshotPath = CreateTempDirectory();
        try
        {
            Repository.Init(repositoryPath);
            using (var repository = new Repository(repositoryPath))
            {
                File.WriteAllText(Path.Combine(repositoryPath, "seed.txt"), "seed");
                Commands.Stage(repository, "*");
                var signature = new Signature("tester", "tester@example.org", new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero));
                repository.Commit("initial", signature, signature);
            }

            File.WriteAllText(Path.Combine(snapshotPath, "seed.txt"), "changed");

            await using var provider = new GitProvider();
            await provider.InitializeTargetAsync(new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = repositoryPath, BranchOrTrunk = "main" }, true, CancellationToken.None);
            await provider.CommitSnapshotAsync(snapshotPath, new CommitMetadata
            {
                Message = "import",
                AuthorName = "alice",
                AuthorEmail = "alice@example.org",
                Timestamp = new DateTimeOffset(2024, 1, 2, 4, 0, 0, TimeSpan.Zero),
                TargetBranch = "main"
            }, CancellationToken.None);

            var commitId = await provider.GetHeadCommitIdAsync(CancellationToken.None);
            Assert.IsFalse(string.IsNullOrWhiteSpace(commitId));

            await provider.EnsureTagAsync("v1.0", commitId!, CancellationToken.None);
            await provider.EnsureTagAsync("v1.0", commitId!, CancellationToken.None);
            await provider.EnsureBranchAsync("releases/1.0", commitId!, CancellationToken.None);
            await provider.EnsureBranchAsync("releases/1.0", commitId!, CancellationToken.None);

            Assert.IsTrue(await provider.TagExistsAsync("v1.0", CancellationToken.None));
            Assert.IsTrue(await provider.BranchExistsAsync("releases/1.0", CancellationToken.None));
        }
        finally
        {
            if (Directory.Exists(repositoryPath))
                TryDeleteDirectory(repositoryPath);
            if (Directory.Exists(snapshotPath))
                TryDeleteDirectory(snapshotPath);
        }
    }

    private static string CreateTempDirectory()
    {
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectoryPath);
        return tempDirectoryPath;
    }

    private static void TryDeleteDirectory(string directoryPath)
    {
        try
        {
            foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
                File.SetAttributes(filePath, FileAttributes.Normal);

            Directory.Delete(directoryPath, recursive: true);
        }
        catch
        {
        }
    }
}
